function storedProcedure(streamId, documentsToCreate, expectedPosition) {

    var context = getContext();
    var collection = context.getCollection();
    var response = context.getResponse();
    var streamType = "Stream";
    var eventType = "Event";

    function checkError(err) {
        if (err) throw new Error("Error : " + err.message);
    }

    function checkErrorFn(err, __) {
        checkError(err);
    }

    function checkPosition(nextPosition) {
        if (expectedPosition.mode == "any") {
            return;
        }
        if (expectedPosition.mode == "noStream" && nextPosition > 1) {
            throw "ESERROR_POSITION_STREAMEXISTS";
        }
        if (expectedPosition.mode == "exact" && nextPosition != expectedPosition.position) {
            throw "ESERROR_POSITION_POSITIONNOTMATCH";
        }
    }

    // append event
    function createDocument(err, metadata) {
        if (err) throw new Error("Error" + err.message);
        checkPosition(metadata.LastPosition + 1);
        var nextPosition = metadata.LastPosition;
        var resp = [];
        for (var i in documentsToCreate) {
            nextPosition++;
            var d = documentsToCreate[i];
            var created = new Date().toISOString();
            var doc = {
                "Type": eventType,
                "id": d.id,
                "CorrelationId": d.correlationId,
                "StreamId": streamId,
                "Position": nextPosition,
                "Name": d.name,
                "Data": d.data,
                "Metadata": d.metadata,
                "CreatedUtc": created
            }

            resp.push({ position: nextPosition, created: created });
            var acceptedDoc = collection.createDocument(collection.getSelfLink(), doc, checkErrorFn);
            if (!acceptedDoc) {
                throw "Failed to append event on position " + nextPosition + " - Rollback. Please try to increase RU for collection Events.";
            }
        }

        metadata.LastPosition = nextPosition;
        metadata.LastUpdatedUtc = created;
        var acceptedMeta = collection.replaceDocument(metadata._self, metadata, checkErrorFn);
        if (!acceptedMeta) {
            throw "Failed to update metadata for stream - Rollback";
        }
        response.setBody(resp);
    }

    // main function
    function run(err, metadataResults) {
        checkError(err);
        if (metadataResults.length == 0) {
            let newMeta = {
                StreamId: streamId,
                Type: streamType,
                LastPosition: 0
            }
            return collection.createDocument(collection.getSelfLink(), newMeta, createDocument);
        } else {
            return createDocument(err, metadataResults[0]);
        }
    }

    var metadataQuery = {
        'query': 'SELECT * FROM AssetEvents e WHERE e.StreamId = @streamId AND e.Type = @streamType',
        'parameters': [{ 'name': '@streamId', 'value': streamId }, { 'name': '@streamType', 'value': streamType }]
    };

    var transactionAccepted = collection.queryDocuments(collection.getSelfLink(), metadataQuery, run);
    if (!transactionAccepted) throw "Transaction not accepted, rollback";
}
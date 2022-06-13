# Portfolio-tracker

## Introduction

Portfolio-tracker is web application wsystem for dynamic capital valuation.

## Technologies

Target frameworks: net6, Angular 13
Database : MSSQL, CosmosDB

## Launch
In order to launch app locally you need to have:

Tools:
Visual Studio 2022 IDE;
MS SQL;
Azure Cosmos DB Emulator;
Azure Storage Emulator;
Angular CLI

Plus you need NServiceBus with queue named: "snapshot-trigger"
You also will need free account here: https://polygon.io/

Please insert connection strings (CosmosDb, MSSQL,NServiceBus) to the projects: PortfolioTracker.Api, PortfolioTracker.SnapshotGenerator, PortfolioTracker.SnapshotTrigger
Please insert token for https://polygon.io/ to the: PortfolioTracker.Market.Api

Set multiple startup projects and run: PortfolioTracker.Market.Api, ortfolioTracker.Api, PortfolioTracker.SnapshotGenerator, PortfolioTracker.SnapshotTrigger

Go to the PortfolioTracker.Ui:
run console command: npm install 
then commans: ng serve --o
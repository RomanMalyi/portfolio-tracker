If not exists(select 1 from sys.tables 
   inner join sys.schemas ON schemas.schema_id = sys.tables.schema_id
   where concat(sys.schemas.name, '.', sys.tables.name) = 'dbo.Account')
begin
Create table [dbo].[Account]
(
	[Id] nvarchar(36) primary key not null,
	[UserId] nvarchar(36) not null,
	[Name] nvarchar(128) not null,
	[AccountType] nvarchar(128) not null,
	[CreatedAt] datetime2 not null,
);
end

If not exists(select 1 from sys.tables 
   inner join sys.schemas ON schemas.schema_id = sys.tables.schema_id
   where concat(sys.schemas.name, '.', sys.tables.name) = 'dbo.Asset')
begin
Create table [dbo].[Asset]
(
	[Id] nvarchar(36) primary key not null,
	[AccountId] nvarchar(36) not null foreign key references [dbo].[Account](Id),
	[UserId] nvarchar(36) not null,
	[Name] nvarchar(128) not null,
	[AssetType] nvarchar(128) not null,
	[ExchangeTicker] nvarchar(128),
	[OpenPrice] decimal,
	[InterestRate] float,
	[Units] decimal not null,
	[Currency] nvarchar(128) not null,
	[RiskLevel] nvarchar(128) not null,
	[CreatedAt] datetime2 not null,
);
end

If not exists(select 1 from sys.tables 
   inner join sys.schemas ON schemas.schema_id = sys.tables.schema_id
   where concat(sys.schemas.name, '.', sys.tables.name) = 'dbo.Transaction')
begin
Create table [dbo].[Transaction]
(
	[Id] nvarchar(36) primary key not null,
	[AssetId] nvarchar(36) not null foreign key references [dbo].[Asset](Id),
	[UserId] nvarchar(36) not null,
	[TransactionType] nvarchar(128) not null,
	[TransactionDate] datetime2 not null,
	[Amount] decimal,
	[FromAssetId] nvarchar(36) foreign key references [dbo].[Asset](Id),
	[ToAssetId] nvarchar(36) foreign key references [dbo].[Asset](Id),
	[ExchangeRate] decimal,
	[Description] nvarchar(400),
	[CreatedAt] datetime2 not null,
);
end
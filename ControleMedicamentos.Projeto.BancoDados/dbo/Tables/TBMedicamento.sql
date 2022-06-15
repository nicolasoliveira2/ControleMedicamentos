CREATE TABLE [dbo].[TBMedicamento] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [Nome]                 VARCHAR (300) NULL,
    [Descricao]            VARCHAR (MAX) NULL,
    [Lote]                 VARCHAR (300) NULL,
    [Validade]             DATE          NULL,
    [QuantidadeDisponivel] INT           NULL,
    [Fornecedor_Id]         INT           NULL,
    CONSTRAINT [PK_TBMedicamento] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TBMedicamento_TBFornecedor] FOREIGN KEY ([Fornecedor_Id]) REFERENCES [dbo].[TBFornecedor] ([Id])
);


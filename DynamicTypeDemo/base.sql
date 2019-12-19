SET IDENTITY_INSERT [dbo].[TableTemplates] ON
INSERT INTO [dbo].[TableTemplates] ([Id], [Name], [IsDelete]) VALUES (3005, N'Metadata', 0)
SET IDENTITY_INSERT [dbo].[TableTemplates] OFF

SET IDENTITY_INSERT [dbo].[TableTemplateFields] ON
INSERT INTO [dbo].[TableTemplateFields] ([Id], [TableTemplateId], [Name], [Title], [Type], [IsKey], [IsRequire], [Length], [IsSysField]) VALUES (2002, 3005, N'F_NAME', N'字段名', 2, 0, 0, 20, 0)
INSERT INTO [dbo].[TableTemplateFields] ([Id], [TableTemplateId], [Name], [Title], [Type], [IsKey], [IsRequire], [Length], [IsSysField]) VALUES (2003, 3005, N'F_TITLE', N'标题', 2, 0, 0, 20, 0)
INSERT INTO [dbo].[TableTemplateFields] ([Id], [TableTemplateId], [Name], [Title], [Type], [IsKey], [IsRequire], [Length], [IsSysField]) VALUES (2004, 3005, N'F_TYPE', N'类型', 1, 0, 0, 20, 0)
INSERT INTO [dbo].[TableTemplateFields] ([Id], [TableTemplateId], [Name], [Title], [Type], [IsKey], [IsRequire], [Length], [IsSysField]) VALUES (2005, 3005, N'F_LEGNTH', N'字段长度', 1, 0, 0, 20, 0)
INSERT INTO [dbo].[TableTemplateFields] ([Id], [TableTemplateId], [Name], [Title], [Type], [IsKey], [IsRequire], [Length], [IsSysField]) VALUES (2006, 3005, N'F_TOOLTIP', N'提示信息', 2, 0, 0, 100, 0)
INSERT INTO [dbo].[TableTemplateFields] ([Id], [TableTemplateId], [Name], [Title], [Type], [IsKey], [IsRequire], [Length], [IsSysField]) VALUES (2007, 3005, N'SYS_ID', N'编号', 1, 1, 0, 20, 0)
INSERT INTO [dbo].[TableTemplateFields] ([Id], [TableTemplateId], [Name], [Title], [Type], [IsKey], [IsRequire], [Length], [IsSysField]) VALUES (2008, 3005, N'F_ISKEY', N'主键', 1, 0, 0, 20, 0)
SET IDENTITY_INSERT [dbo].[TableTemplateFields] OFF

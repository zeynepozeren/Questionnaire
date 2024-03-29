

CREATE TABLE [dbo].[QUESTION](
	[QID] [int] IDENTITY(1,1) NOT NULL,
	[QIDENTIFIER] [nvarchar](10) NOT NULL,
	[DESCRIPTION] [nvarchar](250) NOT NULL,
	[CHOICETYPE] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[QID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




CREATE TABLE [dbo].[CHOICE](
	[CID] [int] IDENTITY(1,1) NOT NULL,
	[QID] [int] NOT NULL,
	[CIDENTIFIER] [nvarchar](10) NOT NULL,
	[DESCRIPTION] [nvarchar](250) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CHOICE]  WITH CHECK ADD FOREIGN KEY([QID])
REFERENCES [dbo].[QUESTION] ([QID])
GO




CREATE TABLE [dbo].[ANSWERS](
	[CID] [int] NOT NULL,
	[RESPONDID] [nvarchar](50) NOT NULL,
	[DATETIME] [datetime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ANSWERS]  WITH CHECK ADD FOREIGN KEY([CID])
REFERENCES [dbo].[CHOICE] ([CID])
GO





INSERT INTO QUESTION SELECT '1','How did you find out about this job opportunity?','0' 
INSERT INTO CHOICE SELECT 1,'A','StackOverflow'
INSERT INTO CHOICE SELECT 1,'B','Indeed'
INSERT INTO CHOICE SELECT 1,'C','Other'

INSERT INTO QUESTION SELECT '2','How do you find the company�s location?','1' 
INSERT INTO CHOICE SELECT 2,'A','Easy to access by public transport'
INSERT INTO CHOICE SELECT 2,'B','Easy to access by car'
INSERT INTO CHOICE SELECT 2,'C','In a pleasant area'
INSERT INTO CHOICE SELECT 2,'D','None of the above'

INSERT INTO QUESTION SELECT '3',' What was your impression of the office where you had the interview?','0' 
INSERT INTO CHOICE SELECT 3,'A','Tidy'
INSERT INTO CHOICE SELECT 3,'B','Sloppy'
INSERT INTO CHOICE SELECT 3,'C','Did not notice'

INSERT INTO QUESTION SELECT '4','How technically challenging was the interview?','0' 
INSERT INTO CHOICE SELECT 4,'A','Very difficult'
INSERT INTO CHOICE SELECT 4,'B','Difficult'
INSERT INTO CHOICE SELECT 4,'C','Moderate'
INSERT INTO CHOICE SELECT 4,'D','Easy'

INSERT INTO QUESTION SELECT '5','How can you describe the manager that interviewed you?','1' 
INSERT INTO CHOICE SELECT 5,'A','Enthusiastic'
INSERT INTO CHOICE SELECT 5,'B','Polite'
INSERT INTO CHOICE SELECT 5,'C','Organized'
INSERT INTO CHOICE SELECT 5,'D','Could not tell'

CREATE TABLE Users(
	ID INT NOT NULL AUTO_INCREMENT,
	UserName NVARCHAR(256) NOT NULL,
	Email NVARCHAR(256) NOT NULL,
	Profile INT NOT NULL Default 2,
	Password NVARCHAR(256) NOT NULL,
	CONSTRAINT PK_User PRIMARY KEY (ID)
);

CREATE TABLE News(
	ID INT NOT NULL AUTO_INCREMENT,
	Date DATETIME(6) NOT NULL,
	Title NVARCHAR(256) NOT NULL,
	Resume NVARCHAR(1024) NOT NULL,
	Detail NVARChAR(5120) NOT NULL,
	CONSTRAINT PK_New PRIMARY KEY (ID)
);

CREATE TABLE Agenda(
	ID INT NOT NULL AUTO_INCREMENT,
	Date DATETIME(6) NOT NULL,
	Title NVARCHAR(256) NOT NULL,
	Resume NVARCHAR(1024) NOT NULL,
	Detail NVARChAR(5120) NOT NULL,
	Address NVARCHAR(256) NULL,
	CONSTRAINT PK_Agenda PRIMARY KEY (ID)
);

CREATE Table EjtPerson(
	ID INT NOT NULL AUTO_INCREMENT,
	Type INT NOT NULL,
	Name NVARCHAR(256) NOT NULL,
	Role NVARCHAR(256) NOT NULL,
	Detail NVARCHAR(256) NOT NULL,
	PhotoName NVARCHAR(256) NOT NULL,
	CONSTRAINT PK_EjtPersonn PRIMARY KEY (ID)
);

CREATE Table EjtAdherent(
	ID INT NOT NULL AUTO_INCREMENT,
	FirstName NVARCHAR(256) NOT NULL,
	LastName NVARCHAR(256) NOT NULL,
	Birthday DATETIME(6) NOT NULL,
	LicenceCode NVARCHAR(50) NOT NULL,
	Weight FLOAT NULL,
	Belt INT NULL,
	CONSTRAINT PK_EjtAdherent PRIMARY KEY (ID)
);

CREATE TABLE EjtAdherentLinkUser(
	ID INT NOT NULL AUTO_INCREMENT,	
	EjtAdherent_ID INT NOT NULL,
	User_ID INT NOT NULL,
	CONSTRAINT PK_EjtAdherentLinkUser PRIMARY KEY (ID),	
	CONSTRAINT FK_EjtAdherent FOREIGN KEY (EjtAdherent_ID) REFERENCES EjtAdherent(ID),
	CONSTRAINT FK_Users FOREIGN KEY (User_ID) REFERENCES Users(ID)
);

CREATE TABLE Competitions(
	ID INT NOT NULL AUTO_INCREMENT,
	Year INT NOT NULL,
	Month INT NOT NULL,
	Day INT NOT NULL,
	Name NVARCHAR(256) NOT NULL,
	Address NVARCHAR(256) NULL,
	YearBirthdayMin INT NOT NULL,
	YearBirthdayMax INT NOT NULL,
	MaxInscriptionDate Date NOT NULL,
	CONSTRAINT PK_Competitions PRIMARY KEY (ID)
);

CREATE Table CompetitionsResult(
	ID INT NOT NULL AUTO_INCREMENT,
	Competition_ID INT NOT NULL,
	Adherent_ID INT NOT NULL,
	Position INT NULL,
	CONSTRAINT PK_CompetitionsResult PRIMARY KEY (ID),
	CONSTRAINT FK_Competitions FOREIGN KEY (Competition_ID) REFERENCES Competitions (ID),
	CONSTRAINT FK_Adherents FOREIGN KEY (Adherent_ID) REFERENCES EjtAdherent (ID)
);

CREATE Table Stages (
	ID INT NOT NULL AUTO_INCREMENT,
	StartDate Date NOT NULL,
	EndDate Date NOT NULL,
	Name NVARCHAR(256) NOT NULL,
	Address NVARCHAR(256) NULL,
	YearBirthdayMin INT NOT NULL,
	YearBirthdayMax INT NOT NULL,
	MaxInscriptionDate Date NOT NULL,
	CONSTRAINT PK_Stages PRIMARY KEY (ID)
);

CREATE Table StagesInscriptions(
	ID INT NOT NULL AUTO_INCREMENT,
	Stages_ID INT NOT NULL,
	Adherent_ID INT NOT NULL,
	CONSTRAINT PK_StagesInscriptions PRIMARY KEY (ID),
	CONSTRAINT FK_Stages FOREIGN KEY (Stages_ID) REFERENCES Stages (ID),
	CONSTRAINT FK_Adherents FOREIGN KEY (Adherent_ID) REFERENCES EjtAdherent (ID)
);

CREATE Table files(
	ID INT NOT NULL AUTO_INCREMENT,
	Path NVARCHAR(256) NOT NULL,
	filename NVARCHAR(256) NOT NULL,
	file LONGBLOB NOT NULL,
	CONSTRAINT PK_files PRIMARY KEY (ID)
);

CREATE Table clothes(
	ID INT NOT NULL AUTO_INCREMENT,
	Ref NVARCHAR(256) NOT NULL,
	TargetPoeple NVARCHAR(256) NOT NULL,
	JacketDescription NVARCHAR(1024) NULL,
	PantDescription NVARCHAR(1024) NULL,
	Description NVARCHAR(1024) NULL,
	Composition NVARCHAR(256) NOT NULL,
	File LONGBLOB NOT NULL,
	Price INT NOT NULL,
	Size NVARCHAR(256) NOT NULL,
	CONSTRAINT PK_clothes PRIMARY KEY (ID)
);

CREATE table clothesOrderDate(
	ID INT NOT NULL AUTO_INCREMENT,
	Date DATETIME(6) NOT NULL,
	CONSTRAINT PK_clothesOrderDate PRIMARY KEY (ID)
);

CREATE table clothesOrder(
	ID INT NOT NULL AUTO_INCREMENT,
	Email NVARCHAR(256) NOT NULL,
	Ref NVARCHAR(256) NOT NULL,	
	Price INT NOT NULL,
	IsPay TINYINT(1) NOT NULL DEFAULT 0,
	IsDelete TINYINT(1) NOT NULL DEFAULT 0,
	CONSTRAINT PK_clothesOrder PRIMARY KEY (ID)
);

CREATE table clothesOrderItem(
	ID INT NOT NULL AUTO_INCREMENT,
	Order_ID INT NOT NULL,
	Ref NVARCHAR(256) NOT NULL,
	Size NVARCHAR(256) NOT NULL,
	Quantity INT NOT NULL,
	Price INT NOT NULL,
	CONSTRAINT PK_clothesOrderItem PRIMARY KEY (ID),
	CONSTRAINT FK_clothesOrder FOREIGN KEY (Order_ID) REFERENCES clothesOrder (ID)
);

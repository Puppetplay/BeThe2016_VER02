﻿DROP TABLE Player_W;
CREATE TABLE Player_W
(
	Id						BIGINT					NOT NULL		PRIMARY KEY	IDENTITY,
	Href					NVARCHAR(1000)			NOT NULL,
	Team					CHAR(2),
	InsertDateTime			DATETIME				DEFAULT			CURRENT_TIMESTAMP
);

DROP TABLE Player;
CREATE TABLE Player
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	PlayerId				INT				NOT NULL			UNIQUE,
	Team					NVARCHAR(10)	NOT NULL,
	BackNumber				INT				NULL,
	Name					NVARCHAR(10)	NOT NULL,
	Height					INT				NOT NULL,
	Weight					INT				NOT NULL,
	Position				NVARCHAR(10)	NOT NULL,
	Hand					NVARCHAR(10)	NOT NULL,
	BirthDate				CHAR(8)			NOT NULL,
	Career					NVARCHAR(50)	NULL,
	Deposit					NVARCHAR(50)	NULL,
	Salary					NVARCHAR(20)	NULL,
	Rank					NVARCHAR(50)	NULL,
	JoinYear				NVARCHAR(20)	NULL,
	SCR						NVARCHAR(70)	NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);

DROP TABLE Schedule;
CREATE TABLE Schedule
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,

	Year					INTEGER			NOT NULL,
	Month					INTEGER			NOT NULL,
	Day						INTEGER			NOT NULL,

	Hour 					INTEGER			NULL,
	Minute 					INTEGER			NULL,

	BallPark				NVARCHAR(10)		NULL,

	HomeTeam				NVARCHAR(10)		NOT NULL,
	AwayTeam				NVARCHAR(10)		NOT NULL,

	HomeTeamScore			INTEGER			NULL,
	AwayTeamScore			INTEGER			NULL,

	Href					NVARCHAR(200)	NULL,
	GameId					NCHAR(13)		NULL,
	LeagueId				INT				NULL,
	SeriesId				INT				NULL,
	
	Etc						NVARCHAR(1000)	NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);

DROP TABLE Situation_W;
CREATE TABLE Situation_W
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	GameId					NCHAR(13)		NOT NULL,
	Content					TEXT			NOT NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);

DROP TABLE BoxScore_W
CREATE TABLE BoxScore_W
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	GameId					NCHAR(13)		NOT NULL,
	AwayHitter				TEXT			NOT NULL,
	HomeHitter				TEXT			NOT NULL,
	AwayPitcher				TEXT			NOT NULL,
	HomePitcher				TEXT			NOT NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
)

DROP TABLE Ball
CREATE TABLE Ball
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	BatId					BIGINT			NOT NULL,
	Number					INT				NULL,
	Speed					INT				NULL,	
	BallType				NCHAR(10)		NOT NULL,
	Result					NCHAR(10)		NOT NULL,
	BallCount				NCHAR(3)		NOT NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);

DROP TABLE Bat
CREATE TABLE Bat
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	ThId					BIGINT			NOT NULL,
	PitcherId				INT				NOT NULL,
	HitterId				INT				NOT NULL,	
	Result					NCHAR(10)		NOT NULL,
	DetailResult			NVARCHAR(100)	NOT NULL,
	PResult					INT				NOT NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);


DROP TABLE Th
CREATE TABLE Th
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	MatchId					BIGINT			NOT NULL,
	ThType					INT				NOT NULL,
	Number					INT				NOT NULL,	
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);

DROP TABLE Match
CREATE TABLE Match
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	GameId					NCHAR(13)		NOT NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);

DROP TABLE LineUp
CREATE TABLE LineUp
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	MatchId					BIGINT			NOT NULL,
	AttackType				INT				NOT NULL,
	PlayerId				INT				NOT NULL,
	BatNumber				INT				NOT NULL,	
	EntryType				INT				NOT NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);

CREATE INDEX IDX_TH_MATCHID ON TH(MATCHID);
CREATE INDEX IDX_BAT_THID ON BAT(THID);

CREATE INDEX IDX_BAT_PITCHERID ON BAT(PITCHERID);
CREATE INDEX IDX_BAT_PRESULT ON BAT(PRESULT);
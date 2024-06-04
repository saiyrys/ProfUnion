DROP DATABASE IF EXISTS `Profunion`;
CREATE DATABASE Profunion;

use Profunion;

CREATE TABLE Users(
userId varchar(255) not null Primary Key,
userName varchar(255) UNIQUE not null,
firstName varchar(255) not null,
lastName varchar(255) not null,
middleName varchar(255) not null,
email Varchar(255) Not Null,
password varchar(255) not null,
salt varchar(255) not null,
role ENUM('USER', 'ADMIN')  NOT NULL DEFAULT("USER"),
createdAt TIMESTAMP,
updatedAt TIMESTAMP
);

CREATE TABLE Categories(
id varchar(255) primary key not null,
name varchar(255),
color ENUM('default')  NOT NULL DEFAULT("default")
);

CREATE TABLE Events(
eventId varchar(255) not null Primary Key,
title varchar(255) Not null,
description Varchar(500) NOT NULL,
eventDate timestamp DEFAULT CURRENT_TIMESTAMP,
link varchar(255) NOT NULL,
totalTickets int NOT NULL,
createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
updatedAt TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE EventCategories(
categoriesId varchar(255),
eventId varchar(255),
foreign key (categoriesId) REFERENCES Categories(id),
	FOREIGN KEY (eventId) REFERENCES Events(eventId)
);
CREATE TABLE News(
newsId varchar(266) primary key
);
CREATE TABLE Comments(
id  varchar(255) primary key not null,
content Varchar(2250) not null,
userId varchar(500) not null, 
createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
FOREIGN KEY (userId) REFERENCES Users(userId)
);

CREATE TABLE Report(
id varchar(255) primary key not null,
content Varchar(2250) not null,
userId varchar(500) not null, 
createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
FOREIGN KEY (userId) REFERENCES Users(userId)
);

CREATE TABLE Application(
id varchar(255) primary key not null,
eventId Varchar(255) not null,
userId varchar(255) not null, 
ticketsCount int Not NULL,
status ENUM("APPROVED", "PENDING", "REJECTED") NOT NULL,
createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
FOREIGN KEY (userId) REFERENCES Users(userId),
	FOREIGN KEY (eventId) REFERENCES Events(eventId)
);
CREATE TABLE RejectedApplication(
Id varchar(255) primary key,
userId VARCHAR(255) not null,
eventId VARCHAR(255) not null,
ticketsCount int not null,
createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
FOREIGN KEY (userId) REFERENCES users(userId),
	FOREIGN KEY (eventId) REFERENCES Events(eventId)
);

CREATE TABLE ReservationList(
Id varchar(255) primary Key not Null,
userId varchar(255) not null,
eventId Varchar(255) not null,
ticketsCount int not null,
createdAt TIMESTAMP DEFAULT current_timestamp,
FOREIGN KEY (userId) REFERENCES users(userId),
	FOREIGN KEY (eventId) REFERENCES Events(eventId)
);

CREATE TABLE Uploads(
id int auto_increment primary key,
fileName varchar(255),
filePath varchar(255)
);

CREATE TABLE EntityUploads(
eventId varchar(255),
userId varchar(255),
newsId varchar(255),
fileId int,
FOREIGN KEY (eventId) REFERENCES Events(eventId ),
    FOREIGN KEY (userId) REFERENCES Users(userId),
			FOREIGN KEY (fileId) REFERENCES Uploads(id),
				FOREIGN KEY (newsId) REFERENCES News(newsId)
);







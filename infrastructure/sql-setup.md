# SQL Server Setup

## Create Database

CREATE DATABASE training;

## Create Users Table

CREATE TABLE Users (
    UserId NVARCHAR(100) PRIMARY KEY,
    PasswordHash NVARCHAR(500) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Permissions NVARCHAR(200) NOT NULL
);

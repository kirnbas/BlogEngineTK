CREATE DATABASE [BlogEngineTK]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BlogEngineTK', FILENAME = N'C:\Users\user_1\\BlogEngineTK.mdf' , SIZE = 5120KB , FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'BlogEngineTK_log', FILENAME = N'C:\Users\user_1\\BlogEngineTK_log.ldf' , SIZE = 1024KB , FILEGROWTH = 10%)
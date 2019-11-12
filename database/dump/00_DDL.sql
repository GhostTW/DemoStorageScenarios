CREATE USER 'dbowner'@'%' IDENTIFIED BY 'pass.123';
GRANT All privileges ON *.* TO 'dbowner'@'%';

CREATE DATABASE IF NOT EXISTS TestDB;
use TestDB;

-- User --

CREATE TABLE `User` (
  `AccountId`            int AUTO_INCREMENT NOT NULL,
  `Code`          varchar(20)        NOT NULL,
  `Password`      varchar(70)        NOT NULL,
  `IsActive`      bit                NOT NULL,
  UNIQUE (`Code`),
  PRIMARY KEY (`AccountId`)
);

CREATE INDEX IX_User_Code ON User (Code);

-- -- Create User --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_CreateUser`(
  IN  IN_Code         varchar(20),
  IN  IN_Password     varchar(70),
  IN  IN_IsActive     bit,
  OUT OUT_ReturnValue int
)
BEGIN
  SET OUT_ReturnValue = 0;
  INSERT INTO User (Code, Password, IsActive) values (IN_Code, IN_Password, IsActive);
  SET OUT_ReturnValue = 1;
END ;;
DELIMITER ;

-- Get Users --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetUsers`(OUT OUT_ReturnValue int)
BEGIN
  SET OUT_ReturnValue = 0;
  SELECT AccountId, Code, Password, IsActive FROM User;
  SET OUT_ReturnValue = 1;
END ;;
DELIMITER ;

-- Get User --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetUser`(
  IN  IN_AccountId           int,
  OUT OUT_ReturnValue int
)
BEGIN
    SET OUT_ReturnValue = 0;
    SELECT AccountId, Code, Password, IsActive FROM User WHERE AccountId = IN_AccountId LIMIT 1;
    SET OUT_ReturnValue = 1;
END ;;
DELIMITER ;

-- Get User By Condition --
DELIMITER ;;

DROP PROCEDURE IF EXISTS `sp_User_GetUsersByCondition`;;

CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetUsersByCondition`(
  IN  IN_Code         varchar(20),
  OUT OUT_ReturnValue INT
)
BEGIN
  SET OUT_ReturnValue = 0;
  SELECT AccountId, Code, Password, IsActive FROM User
  WHERE (IN_Code IS NULL OR Code LIKE CONCAT(IN_Code,'%'));
  SET OUT_ReturnValue = 1;
END;;

DELIMITER ;
-- Update User --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_UpdateUser`(
  IN  IN_AccountId    int,
  OUT OUT_ReturnValue int
)
BEGIN
    SET OUT_ReturnValue = 0;
    UPDATE User SET IsActive = IN_IsActive WHERE AccountId = IN_AccountId;
    SET OUT_ReturnValue = 1;
  END ;;
DELIMITER ;

-- Delete User --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_DeleteUser`(
  IN  IN_AccountId    int,
  OUT OUT_ReturnValue int
)
BEGIN
  SET OUT_ReturnValue = 0;
  DELETE FROM User WHERE AccountId = IN_AccountId;
  SET OUT_ReturnValue = 1;
END ;;
DELIMITER ;

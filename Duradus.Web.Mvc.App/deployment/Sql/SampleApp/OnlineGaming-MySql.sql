
CREATE  DATABASE IF NOT EXISTS `{0}`;
CREATE USER '{1}'@'%'  IDENTIFIED BY  '{2}';
GRANT ALL ON {0}.* TO '{1}'@'%'

use `{0}`;


CREATE TABLE `order_status` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 DEFAULT NULL,
  `Ordinal` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE `product_type` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 DEFAULT NULL,
  `Ordinal` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `user` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(250) DEFAULT NULL,
  `password` varchar(250) DEFAULT NULL,
  `name` varchar(250) DEFAULT NULL,
  `email` varchar(250) DEFAULT NULL,
  `is_approved` bit(1) DEFAULT NULL,
  `details` varchar(4000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 
CREATE TABLE `product` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(250) DEFAULT NULL,
  `memory_spec` float DEFAULT NULL,
  `num_of_playes` float DEFAULT NULL,
  `description` varchar(4000) DEFAULT NULL,
  `price` float DEFAULT NULL,
  `type_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`),
  KEY `FK_ProductType_Product_idx` (`type_Id`),
  CONSTRAINT `FK_ProductType_Product` FOREIGN KEY (`type_Id`) REFERENCES `product_type` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE `user_order` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `order_date` datetime DEFAULT NULL,
  `user_Id` int(11) DEFAULT NULL,
  `status_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`),
  KEY `FK_User_Product_idx` (`user_Id`),
  KEY `FK_Order_status_user_order_idx` (`status_Id`),
  CONSTRAINT `FK_User_user_order` FOREIGN KEY (`user_Id`) REFERENCES `user` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Order_status_user_order` FOREIGN KEY (`status_Id`) REFERENCES `order_status` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE `order_item` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `order_Id` int(11) DEFAULT NULL,
  `product_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`),
  KEY `FK_Product_Order_item_idx` (`product_Id`),
  KEY `FK_order_Order_item_idx` (`order_Id`),
  CONSTRAINT `FK_Product_Order_item` FOREIGN KEY (`product_Id`) REFERENCES `product` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT `FK__User_Order_Order_item` FOREIGN KEY (`order_Id`) REFERENCES `user_order` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

use `@dbname2`;


LOCK TABLES `order_status` WRITE;

INSERT INTO `order_status` VALUES (1,'Invoiced',NULL),(2,'Shipped',NULL),(3,'Closed',NULL);
UNLOCK TABLES;


--
-- Dumping data for table `product_type`
--

LOCK TABLES `product_type` WRITE;

INSERT INTO `product_type` VALUES (1,'Army',NULL),(2,'Drawing',NULL),(3,'Ball',NULL),(4,'First person sooter',NULL),(5,'Aircraft',NULL),(6,'kids',NULL),(7,'Interactive',NULL),(8,'Guns',NULL),(9,'Mario',NULL);
UNLOCK TABLES;
--
-- Dumping data for table `product`
--

LOCK TABLES `product` WRITE;

INSERT INTO `product` VALUES (1,'Clowns',13.5,10,NULL,18,1),(2,'Stroll In',7.5,25,NULL,10,2),(3,'Zombo Buster',16.5,10,NULL,22,3),(4,'Empire',16.0125,10,NULL,21.35,4),(5,'Big Farm',18.75,25,NULL,25,5),(6,'Wartune',22.5,10,NULL,30,6),(7,'OurWorld',30,10,NULL,40,7),(8,'Tanki Online',17.4375,10,NULL,23.25,8),(9,'Herotopia',29.25,10,NULL,39,9),(10,'Supercar',6.9,5,NULL,9.2,7),(11,'Shadow Kings',60.75,10,NULL,81,6),(12,'Drakensang',7.5,5,NULL,10,8),(13,'8 Ball Pool',10.5,15,NULL,14,4),(14,'MiniSoccer',13.8,30,NULL,18.4,3),(15,'WorldSoccer F',7.2375,10,NULL,9.65,2),(16,'Puzzle Pirates',34.5,25,NULL,46,2),(17,'Chocolate mountain',9.5625,25,NULL,12.75,6),(18,'Apples playground',39.75,10,NULL,53,7),(19,'Rice forest',5.25,25,NULL,7,5),(20,'Ravioli Rage',28.5,30,NULL,38,8),(21,'Poker',14.625,20,NULL,19.5,9),(22,'Hot Pepper! DA',15.7875,10,NULL,21.05,3),(23,'Tomato World',12.75,20,NULL,17,2),(24,'Mozzarella Jam',26.1,10,NULL,34.8,2);

UNLOCK TABLES;





--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;

INSERT INTO `user` VALUES (1,'andrew','555-0101','Andrew','andrew@backand.com','','Joined the company as a sales representative, was promoted to sales manager and was then named vice president of sales.'),(2,'nancy','555-0102','Nancy','nancy@backand.com','\0',NULL),(3,'laura','555-0103','Laura','laura@backand.com','','Reads and writes French.'),(4,'anne','555-0104','Anne','anne@backand.com','','Fluent in French and German.'),(5,'jan','555-0105','Jan','jan@backand.com','','Was hired as a sales associate and was promoted to sales representative.'),(6,'michael','555-0106','Michael','michael@backand.com','','Fluent in Japanese and can read and write French, Portuguese, and Spanish.'),(7,'mariya','555-0107','Mariya','mariya@backand.com','',NULL),(8,'steven','555-0108','Steven','steven@backand.com','','Joined the company as a sales representative and was promoted to sales manager.  Fluent in French.'),(9,'robert','555-0109','Robert','robert@backand.com',NULL,NULL),(12,'laura','555-0103','Laura','laura@backand.com','','Reads and writes French.');

UNLOCK TABLES;




-- Dumping data for table `user_order`
--

LOCK TABLES `user_order` WRITE;

INSERT INTO `user_order` VALUES (1,'2015-02-09 00:00:00',1,1),(2,'2014-11-03 00:00:00',9,3),(3,'2015-02-01 00:00:00',6,1),(4,'2014-09-15 00:00:00',5,1);

UNLOCK TABLES;

LOCK TABLES `order_item` WRITE;

INSERT INTO `order_item` VALUES (1,1,5),(2,1,1),(3,1,9),(4,1,7),(5,2,19),(6,2,2),(7,2,8),(8,2,23),(9,3,24),(10,3,7),(11,3,21),(12,3,16),(13,4,5),(14,4,1),(15,4,23),(16,4,6);

UNLOCK TABLES;
-- Dump completed on 2015-02-15 23:46:26
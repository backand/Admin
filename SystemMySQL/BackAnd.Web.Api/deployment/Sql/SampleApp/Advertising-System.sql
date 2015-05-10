use `__DB__Name__`;

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `advertisement`
--

DROP TABLE IF EXISTS `advertisement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `advertisement` (
  `Id` int(11) NOT NULL,
  `agency_Id` int(11) DEFAULT NULL,
  `marketing_campaign_Id` int(11) DEFAULT NULL,
  `channel_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `advertisement`
--

INSERT INTO `advertisement` VALUES (1,5,2,2);
INSERT INTO `advertisement` VALUES (2,8,3,2);
INSERT INTO `advertisement` VALUES (3,2,1,5);
INSERT INTO `advertisement` VALUES (4,1,4,9);
INSERT INTO `advertisement` VALUES (5,2,6,10);
INSERT INTO `advertisement` VALUES (6,3,2,6);
INSERT INTO `advertisement` VALUES (7,7,3,5);
INSERT INTO `advertisement` VALUES (8,8,7,8);
INSERT INTO `advertisement` VALUES (9,6,7,8);
INSERT INTO `advertisement` VALUES (10,2,2,12);

--
-- Table structure for table `agency`
--

DROP TABLE IF EXISTS `agency`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `agency` (
  `Id` int(11) NOT NULL,
  `name` varchar(250) DEFAULT NULL,
  `description` varchar(4000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `agency`
--

INSERT INTO `agency` VALUES (1,'Sponge Agency',NULL);
INSERT INTO `agency` VALUES (2,'Smart Inc.',NULL);
INSERT INTO `agency` VALUES (3,'Freckle',NULL);
INSERT INTO `agency` VALUES (4,'Zubiad',NULL);
INSERT INTO `agency` VALUES (5,'Newhaven Agency',NULL);
INSERT INTO `agency` VALUES (6,'Crayons',NULL);
INSERT INTO `agency` VALUES (7,'1 Point Size Agency',NULL);
INSERT INTO `agency` VALUES (8,'Beacon Agency',NULL);

--
-- Table structure for table `channel`
--

DROP TABLE IF EXISTS `channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `channel` (
  `Id` int(11) NOT NULL,
  `name` varchar(250) DEFAULT NULL,
  `description` varchar(4000) DEFAULT NULL,
  `channel_type_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channel`
--

INSERT INTO `channel` VALUES (1,'Agillian',NULL,1);
INSERT INTO `channel` VALUES (2,'Agillian',NULL,3);
INSERT INTO `channel` VALUES (3,'Adapt Partners',NULL,1);
INSERT INTO `channel` VALUES (4,'Adapt Partners',NULL,2);
INSERT INTO `channel` VALUES (5,'Adapt Partners',NULL,3);
INSERT INTO `channel` VALUES (6,'Angular',NULL,1);
INSERT INTO `channel` VALUES (7,'Angular',NULL,2);
INSERT INTO `channel` VALUES (8,'Angular',NULL,3);
INSERT INTO `channel` VALUES (9,'Blind Five Year Old',NULL,1);
INSERT INTO `channel` VALUES (10,'Blind Five Year Old',NULL,2);
INSERT INTO `channel` VALUES (11,'Blind Five Year Old',NULL,3);
INSERT INTO `channel` VALUES (12,'Conversion Factory',NULL,4);

--
-- Table structure for table `channel_type`
--

DROP TABLE IF EXISTS `channel_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `channel_type` (
  `Id` int(11) NOT NULL,
  `name` varchar(250) DEFAULT NULL,
  `description` varchar(4000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channel_type`
--

INSERT INTO `channel_type` VALUES (1,'SEO',' Search Engine Optimization');
INSERT INTO `channel_type` VALUES (2,'PPC','Pay Per Click Campaigns');
INSERT INTO `channel_type` VALUES (3,'SMM','Social Media Marketing');
INSERT INTO `channel_type` VALUES (4,'AM','Affiliate Marketing');
INSERT INTO `channel_type` VALUES (5,'DM','Direct marketing');

--
-- Table structure for table `client`
--

DROP TABLE IF EXISTS `client`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `client` (
  `Id` int(11) NOT NULL,
  `Name` varchar(250) CHARACTER SET utf8 DEFAULT NULL,
  `Ordinal` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `client`
--

INSERT INTO `client` VALUES (1,'Signal Gift Stores',NULL);
INSERT INTO `client` VALUES (2,'Australian Collectors, Co.',NULL);
INSERT INTO `client` VALUES (3,'La Rochelle Gifts',NULL);
INSERT INTO `client` VALUES (4,'Baane Mini Imports',NULL);
INSERT INTO `client` VALUES (5,'Atelier graphique',NULL);
INSERT INTO `client` VALUES (6,'Land of Toys Inc.',NULL);
INSERT INTO `client` VALUES (7,'Euro+ Shopping Channel',NULL);
INSERT INTO `client` VALUES (8,'Gift Depot Inc.',NULL);

--
-- Table structure for table `invoice`
--

DROP TABLE IF EXISTS `invoice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `invoice` (
  `Id` int(11) NOT NULL,
  `agency_Id` int(11) DEFAULT NULL,
  `client_Id` int(11) DEFAULT NULL,
  `invoice_status_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoice`
--

INSERT INTO `invoice` VALUES (1,4,7,2);
INSERT INTO `invoice` VALUES (2,5,8,2);
INSERT INTO `invoice` VALUES (3,5,5,3);
INSERT INTO `invoice` VALUES (4,6,1,5);
INSERT INTO `invoice` VALUES (5,8,3,5);
INSERT INTO `invoice` VALUES (6,1,6,6);
INSERT INTO `invoice` VALUES (7,3,2,1);
INSERT INTO `invoice` VALUES (8,2,4,4);
INSERT INTO `invoice` VALUES (9,2,7,4);

--
-- Table structure for table `invoice_status`
--

DROP TABLE IF EXISTS `invoice_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `invoice_status` (
  `Id` int(11) NOT NULL,
  `name` varchar(250) DEFAULT NULL,
  `description` varchar(4000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoice_status`
--

INSERT INTO `invoice_status` VALUES (1,'Cancelled',NULL);
INSERT INTO `invoice_status` VALUES (2,'Disputed',NULL);
INSERT INTO `invoice_status` VALUES (3,'In Process',NULL);
INSERT INTO `invoice_status` VALUES (4,'Resolved',NULL);
INSERT INTO `invoice_status` VALUES (5,'Shipped',NULL);
INSERT INTO `invoice_status` VALUES (6,'On Hold',NULL);

--
-- Table structure for table `items`
--

DROP TABLE IF EXISTS `items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `items` (
  `Id` int(11) NOT NULL,
  `name` varchar(250) DEFAULT NULL,
  `description` varchar(4000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `items`
--

INSERT INTO `items` VALUES (1,'18th century schooner',NULL);
INSERT INTO `items` VALUES (2,'Pont Yacht',NULL);
INSERT INTO `items` VALUES (3,'F/A 18 Hornet 1/72',NULL);
INSERT INTO `items` VALUES (4,'America West Airlines B757-200',NULL);
INSERT INTO `items` VALUES (5,'ATA: B757-300',NULL);
INSERT INTO `items` VALUES (6,'The USS Constitution Ship',NULL);
INSERT INTO `items` VALUES (7,'1982 Camaro Z28',NULL);

--
-- Table structure for table `marketing_campaign`
--

DROP TABLE IF EXISTS `marketing_campaign`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `marketing_campaign` (
  `Id` int(11) NOT NULL,
  `name` varchar(250) DEFAULT NULL,
  `description` varchar(4000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `marketing_campaign`
--

INSERT INTO `marketing_campaign` VALUES (1,'guerrilla-car',NULL);
INSERT INTO `marketing_campaign` VALUES (2,'OMFG',NULL);
INSERT INTO `marketing_campaign` VALUES (3,'Revelation',NULL);
INSERT INTO `marketing_campaign` VALUES (4,'REFRESH Project',NULL);
INSERT INTO `marketing_campaign` VALUES (5,'The Man Your Man Could Drive Like',NULL);
INSERT INTO `marketing_campaign` VALUES (6,'In Rainbows',NULL);
INSERT INTO `marketing_campaign` VALUES (7,'Let\'s Motor',NULL);

--
-- Table structure for table `response`
--

DROP TABLE IF EXISTS `response`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `response` (
  `Id` int(11) NOT NULL,
  `advertisement_Id` int(11) DEFAULT NULL,
  `response_tag_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `response`
--

INSERT INTO `response` VALUES (1,5,2);
INSERT INTO `response` VALUES (2,6,3);
INSERT INTO `response` VALUES (3,7,1);
INSERT INTO `response` VALUES (4,8,2);
INSERT INTO `response` VALUES (5,1,1);
INSERT INTO `response` VALUES (6,3,1);
INSERT INTO `response` VALUES (7,2,3);
INSERT INTO `response` VALUES (8,10,1);
INSERT INTO `response` VALUES (9,4,1);
INSERT INTO `response` VALUES (10,9,3);

--
-- Table structure for table `response_tag`
--

DROP TABLE IF EXISTS `response_tag`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `response_tag` (
  `Id` int(11) NOT NULL,
  `Name` varchar(250) CHARACTER SET utf8 DEFAULT NULL,
  `Ordinal` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `response_tag`
--

INSERT INTO `response_tag` VALUES (1,'Cars',NULL);
INSERT INTO `response_tag` VALUES (2,'Oldies',NULL);
INSERT INTO `response_tag` VALUES (3,'Antique',NULL);
INSERT INTO `response_tag` VALUES (4,'Car Sale',NULL);
INSERT INTO `response_tag` VALUES (5,'Moto Sale',NULL);
INSERT INTO `response_tag` VALUES (6,'Car Buy',NULL);
INSERT INTO `response_tag` VALUES (7,'Boat Buy',NULL);

--
-- Dumping routines for database 'yariv-jsontemplagamycun4'
--

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-04-01 16:24:30

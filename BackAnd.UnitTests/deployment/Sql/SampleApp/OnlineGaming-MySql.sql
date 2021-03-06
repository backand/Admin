

use `__DB__Name__`;


create table `order_item` (`id` int(11) not null auto_increment primary key, `product_Id` int(11), `order_Id` int(11));
create table `order_status` (`id` int(11) not null auto_increment primary key, `Name` varchar(255), `Ordinal` float(8, 2));
create table `product` (`id` int(11) not null auto_increment primary key, `type_Id` int(11), `name` varchar(255), `memory_spec` float(8, 2), `num_of_playes` float(8, 2), `description` text, `price` float(8, 2));
create table `product_type` (`id` int(11) not null auto_increment primary key, `Name` varchar(255), `Ordinal` float(8, 2));
create table `user` (`id` int(11) not null auto_increment primary key, `username` varchar(255), `password` varchar(255), `name` varchar(255), `email` varchar(255), `is_approved` Bit(1), `details` text);
create table `user_order` (`id` int(11) not null auto_increment primary key, `user_Id` int(11), `status_Id` int(11), `order_date` datetime);

alter table `order_item` add constraint order_item_product_id_foreign foreign key (`product_Id`) references `product` (`id`);
alter table `order_item` add constraint order_item_order_id_foreign foreign key (`order_Id`) references `user_order` (`id`);
alter table `product` add constraint product_type_id_foreign foreign key (`type_Id`) references `product_type` (`id`);
alter table `user_order` add constraint user_order_user_id_foreign foreign key (`user_Id`) references `user` (`id`);
alter table `user_order` add constraint user_order_status_id_foreign foreign key (`status_Id`) references `order_status` (`id`);

LOCK TABLES `order_status` WRITE;
INSERT INTO `order_status` VALUES (1,'Invoiced',NULL),(2,'Shipped',NULL),(3,'Closed',NULL);
UNLOCK TABLES;

LOCK TABLES `product_type` WRITE;
INSERT INTO `product_type` VALUES (1,'Army',NULL),(2,'Drawing',NULL),(3,'Ball',NULL),(4,'First person sooter',NULL),(5,'Aircraft',NULL),(6,'kids',NULL),(7,'Interactive',NULL),(8,'Guns',NULL),(9,'Mario',NULL);
UNLOCK TABLES;

LOCK TABLES `user` WRITE;
INSERT INTO `user` (`id`,`username`, `password`, `name`, `email`, `is_approved`, `details`) VALUES (1,'andrew','555-0101','Andrew','andrew@backand.com','','Joined the company as a sales representative, was promoted to sales manager and was then named vice president of sales.'),(2,'nancy','555-0102','Nancy','nancy@backand.com','\0',NULL),(3,'laura','555-0103','Laura','laura@backand.com','','Reads and writes French.'),(4,'anne','555-0104','Anne','anne@backand.com','','Fluent in French and German.'),(5,'jan','555-0105','Jan','jan@backand.com','','Was hired as a sales associate and was promoted to sales representative.'),(6,'michael','555-0106','Michael','michael@backand.com','','Fluent in Japanese and can read and write French, Portuguese, and Spanish.'),(7,'mariya','555-0107','Mariya','mariya@backand.com','',NULL),(8,'steven','555-0108','Steven','steven@backand.com','','Joined the company as a sales representative and was promoted to sales manager.  Fluent in French.'),(9,'robert','555-0109','Robert','robert@backand.com',NULL,NULL),(12,'laura','555-0103','Laura','laura@backand.com','','Reads and writes French.');
UNLOCK TABLES;


LOCK TABLES `product` WRITE;
INSERT INTO `product` (`id`,`name`,`memory_spec`,`num_of_playes`,`description`,`price`,`type_Id`) VALUES (1,'Clowns',13.5,10,NULL,18,1),(2,'Stroll In',7.5,25,NULL,10,2),(3,'Zombo Buster',16.5,10,NULL,22,3),(4,'Empire',16.0125,10,NULL,21.35,4),(5,'Big Farm',18.75,25,NULL,25,5),(6,'Wartune',22.5,10,NULL,30,6),(7,'OurWorld',30,10,NULL,40,7),(8,'Tanki Online',17.4375,10,NULL,23.25,8),(9,'Herotopia',29.25,10,NULL,39,9),(10,'Supercar',6.9,5,NULL,9.2,7),(11,'Shadow Kings',60.75,10,NULL,81,6),(12,'Drakensang',7.5,5,NULL,10,8),(13,'8 Ball Pool',10.5,15,NULL,14,4),(14,'MiniSoccer',13.8,30,NULL,18.4,3),(15,'WorldSoccer F',7.2375,10,NULL,9.65,2),(16,'Puzzle Pirates',34.5,25,NULL,46,2),(17,'Chocolate mountain',9.5625,25,NULL,12.75,6),(18,'Apples playground',39.75,10,NULL,53,7),(19,'Rice forest',5.25,25,NULL,7,5),(20,'Ravioli Rage',28.5,30,NULL,38,8),(21,'Poker',14.625,20,NULL,19.5,9),(22,'Hot Pepper! DA',15.7875,10,NULL,21.05,3),(23,'Tomato World',12.75,20,NULL,17,2),(24,'Mozzarella Jam',26.1,10,NULL,34.8,2);
UNLOCK TABLES;


LOCK TABLES `user_order` WRITE;
INSERT INTO `user_order` (`id`,`order_date`,`user_Id`,`status_Id`) VALUES (1,'2015-02-09 00:00:00',1,1),(2,'2014-11-03 00:00:00',9,3),(3,'2015-02-01 00:00:00',6,1),(4,'2014-09-15 00:00:00',5,1);
UNLOCK TABLES;

LOCK TABLES `order_item` WRITE;
INSERT INTO `order_item` (`id`,`order_Id`,`product_Id`) VALUES (1,1,5),(2,1,1),(3,1,9),(4,1,7),(5,2,19),(6,2,2),(7,2,8),(8,2,23),(9,3,24),(10,3,7),(11,3,21),(12,3,16),(13,4,5),(14,4,1),(15,4,23),(16,4,6);
UNLOCK TABLES;


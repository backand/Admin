use `__DB__Name__`;


create table `campaign_customer` (`id` int(11) not null auto_increment primary key, `customer` int(11), `email_campaign` int(11), `campaign_outcome` int(11));
create table `campaign_outcome` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `customer` (`id` int(11) not null auto_increment primary key, `payment_method` int(11), `name` varchar(255), `phone` varchar(255), `email` varchar(255), `address` varchar(255), `login` varchar(255), `password` varchar(255), `details` text);
create table `customer_order` (`id` int(11) not null auto_increment primary key, `customer` int(11), `order_status` int(11), `create_date` datetime, `shipping_method` int(11), `delivered_date` datetime, `shipping_charges` float(8, 2), `details` text);
create table `email_campaign` (`id` int(11) not null auto_increment primary key, `product_category_code` varchar(255), `campaign_name` varchar(255), `start_date` datetime, `end_date` datetime, `target_population` text, `objective` text);
create table `item_status` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `order_product` (`id` int(11) not null auto_increment primary key, `product` int(11), `customer_order` int(11), `quantity` float(8, 2), `item_status` int(11));
create table `order_status` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `payment_method` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `product` (`id` int(11) not null auto_increment primary key, `product_category` int(11), `name` varchar(255), `details` text);
create table `product_category` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `shipping_method` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `shipping_charges` float(8, 2));
alter table `campaign_customer` add constraint customer_campaign_customer_customer_bkname_campaign_customer foreign key (`customer`) references `customer` (`id`); 
alter table `campaign_customer` add constraint bb4250678f59e41e98b8ce0a4b781e4c3_bkname_campaign_customer foreign key (`email_campaign`) references `email_campaign` (`id`); 
alter table `campaign_customer` add constraint b788612642fb649bab7a182a7ff0c33df_bkname_campaign_customer foreign key (`campaign_outcome`) references `campaign_outcome` (`id`);
alter table `customer` add constraint shipping_method_customer_payment_method_bkname_customer foreign key (`payment_method`) references `shipping_method` (`id`);
alter table `customer_order` add constraint customer_customer_order_customer_bkname_customer_order foreign key (`customer`) references `customer` (`id`); 
alter table `customer_order` add constraint order_status_customer_order_order_status_bkname_customer_order foreign key (`order_status`) references `order_status` (`id`); 
alter table `customer_order` add constraint b6c2493fb980444be915b66b7c52d7d2f_bkname_customer_order foreign key (`shipping_method`) references `shipping_method` (`id`);
alter table `order_product` add constraint product_order_product_product_bkname_order_product foreign key (`product`) references `product` (`id`); 
alter table `order_product` add constraint b44bec89168ab43c580b2944c75367d2b_bkname_order_product foreign key (`customer_order`) references `customer_order` (`id`); 
alter table `order_product` add constraint item_status_order_product_item_status_bkname_order_product foreign key (`item_status`) references `item_status` (`id`);
alter table `product` add constraint product_category_product_product_category_bkname_product foreign key (`product_category`) references `product_category` (`id`);

LOCK TABLES `shipping_method` WRITE;
/*!40000 ALTER TABLE `shipping_method` DISABLE KEYS */;
INSERT INTO `shipping_method` (`id`,`name`,`shipping_charges`) VALUES (1,'UPS',NULL),(2,'USPS Express',NULL),(3,'USPS Priority',NULL),(4,'FedEx',NULL),(5,'UPS',NULL);
/*!40000 ALTER TABLE `shipping_method` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `product_category` WRITE;
/*!40000 ALTER TABLE `product_category` DISABLE KEYS */;
INSERT INTO `product_category` (`id`,`name`,`description`) VALUES (1,'Private Label Distributor',NULL),(2,'Cosmetic',NULL),(3,'Dietary Supplement',NULL),(4,'Drug for Further Processing',NULL),(5,'Medical Food',NULL),(6,'Premarket Notification',NULL);
/*!40000 ALTER TABLE `product_category` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `campaign_outcome` WRITE;
/*!40000 ALTER TABLE `campaign_outcome` DISABLE KEYS */;
INSERT INTO `campaign_outcome` (`id`,`name`,`description`) VALUES (1,'high click',NULL),(2,'low click ',NULL),(3,'lower click ',NULL),(4,'low converation rate',NULL),(5,'high open rate',NULL),(6,'low open rate',NULL);
/*!40000 ALTER TABLE `campaign_outcome` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `item_status` WRITE;
/*!40000 ALTER TABLE `item_status` DISABLE KEYS */;
INSERT INTO `item_status` (`id`,`name`,`description`) VALUES (1,'available',NULL),(2,'not in stock',NULL),(3,'Closed',NULL);
/*!40000 ALTER TABLE `item_status` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `order_status` WRITE;
/*!40000 ALTER TABLE `order_status` DISABLE KEYS */;
INSERT INTO `order_status` (`id`,`name`,`description`) VALUES (1,'Invoiced',NULL),(2,'Shipped',NULL),(3,'Closed',NULL);
/*!40000 ALTER TABLE `order_status` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `payment_method` WRITE;
/*!40000 ALTER TABLE `payment_method` DISABLE KEYS */;
INSERT INTO `payment_method` (`id`,`name`,`description`) VALUES (1,'Visa',NULL),(2,'American Express',NULL),(3,'Mastercard.',NULL);
/*!40000 ALTER TABLE `payment_method` ENABLE KEYS */;
UNLOCK TABLES;




LOCK TABLES `product` WRITE;
/*!40000 ALTER TABLE `product` DISABLE KEYS */;
INSERT INTO `product` (`id`, `name`, `details`,`product_category`) VALUES (1,'Vitamin A',NULL,3),(2,'Vitamin B1',NULL,3),(3,'Calcium',NULL,3),(4,'Ginger',NULL,3),(5,'Sentra AM',NULL,5),(6,'Theramine',NULL,5),(7,'100 Percent Pure Healthy Skin ',NULL,2),(8,'Alima Pure Satin Matte Mineral Foundation ',NULL,2);
/*!40000 ALTER TABLE `product` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `customer` WRITE;
/*!40000 ALTER TABLE `customer` DISABLE KEYS */;
INSERT INTO `customer` (`id`, `name` , `phone` , `email` , `address`, `login` , `password` , `details`,`payment_method`) VALUES (1,'Andrew','555-0101','andrew@backand.com','795 E DRAGRAM','Andrew','123456',NULL,2),(2,'Nancy','555-0102','nancy@backand.com',' 200 E MAIN ST','Nancy','123456',NULL,2),(3,'Laura','555-0103','laura@backand.com',' 300 BOYLSTON AVE E','Laura','123456',NULL,4),(4,'Anne','555-0104','anne@backand.com','100 MAIN ST','Anne','123456',NULL,1);
/*!40000 ALTER TABLE `customer` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `email_campaign` WRITE;
/*!40000 ALTER TABLE `email_campaign` DISABLE KEYS */;
INSERT INTO `email_campaign` (`id`,`product_category_code`, `campaign_name` , `start_date`, `end_date`, `target_population`, `objective`) VALUES (1,'4','Prevention Campaigns','2015-01-01 00:00:00','2016-01-01 00:00:00','Old, Young',NULL),(2,'5','Road to Health','2015-01-01 00:00:00','2016-01-01 00:00:00','Young ,Middle age',NULL),(3,'2','Directory of beauty','2015-01-01 00:00:00','2016-01-01 00:00:00','teenagers to Middle age',NULL);
/*!40000 ALTER TABLE `email_campaign` ENABLE KEYS */;
UNLOCK TABLES;



LOCK TABLES `campaign_customer` WRITE;
/*!40000 ALTER TABLE `campaign_customer` DISABLE KEYS */;
INSERT INTO `campaign_customer` (`id`,`customer`,`email_campaign`,`campaign_outcome`) VALUES (1,2,3,2),(2,4,1,1),(3,1,2,3),(4,3,3,4);
/*!40000 ALTER TABLE `campaign_customer` ENABLE KEYS */;
UNLOCK TABLES;



LOCK TABLES `customer_order` WRITE;
/*!40000 ALTER TABLE `customer_order` DISABLE KEYS */;
INSERT INTO `customer_order` (`id`,`create_date`, `delivered_date` , `shipping_charges` , `details` , `customer` , `order_status` , `shipping_method`) VALUES (1,'2015-01-25 00:00:00','2015-02-25 00:00:00',14,NULL,3,2,3),(2,'2015-02-20 00:00:00','2015-02-25 00:00:00',16,NULL,1,3,2),(3,'2015-01-15 00:00:00','2015-02-05 00:00:00',8,NULL,4,1,4),(4,'2015-01-02 00:00:00','2015-02-01 00:00:00',2,NULL,2,3,4);
/*!40000 ALTER TABLE `customer_order` ENABLE KEYS */;
UNLOCK TABLES;





LOCK TABLES `order_product` WRITE;
/*!40000 ALTER TABLE `order_product` DISABLE KEYS */;
INSERT INTO `order_product` (`id`,`quantity`,`product`,`customer_order`,`item_status`) VALUES (1,20,3,1,1),(2,15,7,3,2),(3,13,5,3,2),(4,25,5,2,2),(5,30,2,1,3);
/*!40000 ALTER TABLE `order_product` ENABLE KEYS */;
UNLOCK TABLES;



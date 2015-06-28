use `__DB__Name__`;

create table `advertisement` (`id` int(11) not null auto_increment primary key, `agency` int(11), `marketing_campaign` int(11), `channel` int(11));
create table `agency` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `channel` (`id` int(11) not null auto_increment primary key, `channel_type` int(11), `name` varchar(255), `description` text);
create table `channel_type` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `client` (`id` int(11) not null auto_increment primary key, `Name` varchar(255), `Ordinal` float(8, 2));
create table `invoice` (`id` int(11) not null auto_increment primary key, `agency` int(11), `client` int(11), `invoice_status` int(11));
create table `invoice_status` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `items` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);
create table `marketing_campaign` (`id` int(11) not null auto_increment primary key, `name` varchar(255), `description` text);

alter table `advertisement` add constraint agency_advertisement_agency_bkname_advertisement foreign key (`agency`) references `agency` (`id`);
alter table `advertisement` add constraint b97e1c70d27254bf8aa4a55227c31c3fb_bkname_advertisement foreign key (`marketing_campaign`) references `marketing_campaign` (`id`);
alter table `advertisement` add constraint channel_advertisement_channel_bkname_advertisement foreign key (`channel`) references `channel` (`id`);
alter table `channel` add constraint channel_type_channel_channel_type_bkname_channel foreign key (`channel_type`) references `channel_type` (`id`);
alter table `invoice` add constraint agency_invoice_agency_bkname_invoice foreign key (`agency`) references `agency` (`id`);
alter table `invoice` add constraint client_invoice_client_bkname_invoice foreign key (`client`) references `client` (`id`);
alter table `invoice` add constraint invoice_status_invoice_invoice_status_bkname_invoice foreign key (`invoice_status`) references `invoice_status` (`id`);


INSERT INTO `agency` VALUES (1,'Sponge Agency',NULL);
INSERT INTO `agency` VALUES (2,'Smart Inc.',NULL);
INSERT INTO `agency` VALUES (3,'Freckle',NULL);
INSERT INTO `agency` VALUES (4,'Zubiad',NULL);
INSERT INTO `agency` VALUES (5,'Newhaven Agency',NULL);
INSERT INTO `agency` VALUES (6,'Crayons',NULL);
INSERT INTO `agency` VALUES (7,'1 Point Size Agency',NULL);
INSERT INTO `agency` VALUES (8,'Beacon Agency',NULL);


INSERT INTO `channel_type` VALUES (1,'SEO',' Search Engine Optimization');
INSERT INTO `channel_type` VALUES (2,'PPC','Pay Per Click Campaigns');
INSERT INTO `channel_type` VALUES (3,'SMM','Social Media Marketing');
INSERT INTO `channel_type` VALUES (4,'AM','Affiliate Marketing');
INSERT INTO `channel_type` VALUES (5,'DM','Direct marketing');

INSERT INTO `client` VALUES (1,'Signal Gift Stores',NULL);
INSERT INTO `client` VALUES (2,'Australian Collectors, Co.',NULL);
INSERT INTO `client` VALUES (3,'La Rochelle Gifts',NULL);
INSERT INTO `client` VALUES (4,'Baane Mini Imports',NULL);
INSERT INTO `client` VALUES (5,'Atelier graphique',NULL);
INSERT INTO `client` VALUES (6,'Land of Toys Inc.',NULL);
INSERT INTO `client` VALUES (7,'Euro+ Shopping Channel',NULL);
INSERT INTO `client` VALUES (8,'Gift Depot Inc.',NULL);

INSERT INTO `invoice_status` VALUES (1,'Cancelled',NULL);
INSERT INTO `invoice_status` VALUES (2,'Disputed',NULL);
INSERT INTO `invoice_status` VALUES (3,'In Process',NULL);
INSERT INTO `invoice_status` VALUES (4,'Resolved',NULL);
INSERT INTO `invoice_status` VALUES (5,'Shipped',NULL);
INSERT INTO `invoice_status` VALUES (6,'On Hold',NULL);

INSERT INTO `items` VALUES (1,'18th century schooner',NULL);
INSERT INTO `items` VALUES (2,'Pont Yacht',NULL);
INSERT INTO `items` VALUES (3,'F/A 18 Hornet 1/72',NULL);
INSERT INTO `items` VALUES (4,'America West Airlines B757-200',NULL);
INSERT INTO `items` VALUES (5,'ATA: B757-300',NULL);
INSERT INTO `items` VALUES (6,'The USS Constitution Ship',NULL);
INSERT INTO `items` VALUES (7,'1982 Camaro Z28',NULL);


INSERT INTO `marketing_campaign` VALUES (1,'guerrilla-car',NULL);
INSERT INTO `marketing_campaign` VALUES (2,'OMFG',NULL);
INSERT INTO `marketing_campaign` VALUES (3,'Revelation',NULL);
INSERT INTO `marketing_campaign` VALUES (4,'REFRESH Project',NULL);
INSERT INTO `marketing_campaign` VALUES (5,'The Man Your Man Could Drive Like',NULL);
INSERT INTO `marketing_campaign` VALUES (6,'In Rainbows',NULL);
INSERT INTO `marketing_campaign` VALUES (7,'Let\'s Motor',NULL);

INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (1,'Agillian',NULL,1);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (2,'Agillian',NULL,3);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (3,'Adapt Partners',NULL,1);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (4,'Adapt Partners',NULL,2);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (5,'Adapt Partners',NULL,3);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (6,'Angular',NULL,1);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (7,'Angular',NULL,2);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (8,'Angular',NULL,3);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (9,'Blind Five Year Old',NULL,1);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (10,'Blind Five Year Old',NULL,2);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (11,'Blind Five Year Old',NULL,3);
INSERT INTO `channel` (`id`,`name`,`description`,`channel_type`) VALUES (12,'Conversion Factory',NULL,4);




INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (1,5,2,2);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (2,8,3,2);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (3,2,1,5);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (4,1,4,9);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (5,2,6,10);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (6,3,2,6);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (7,7,3,5);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (8,8,7,8);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (9,6,7,8);
INSERT INTO `advertisement` (`id`, `agency`,`marketing_campaign`,`channel`) VALUES (10,2,2,12);


INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (1,4,7,2);
INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (2,5,8,2);
INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (3,5,5,3);
INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (4,6,1,5);
INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (5,8,3,5);
INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (6,1,6,6);
INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (7,3,2,1);
INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (8,2,4,4);
INSERT INTO `invoice` (`id`,`agency`,`client`,`invoice_status`) VALUES (9,2,7,4);



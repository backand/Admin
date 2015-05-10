use `__DB__Name__`;

CREATE TABLE `todo` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `description` varchar(250) DEFAULT NULL,
  `completed` bit(1) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

insert into todo (description, completed) values ('learn angular',true); 
insert into todo (description, completed) values ('build an angular app',true); 
insert into todo (description, completed) values ('signup to Backand',true); 
insert into todo (description, completed) values ('add rest api',false); 

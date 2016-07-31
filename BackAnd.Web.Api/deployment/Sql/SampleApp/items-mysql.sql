use `__DB__Name__`;

CREATE TABLE `items` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(250) DEFAULT NULL,
 `description` varchar(4000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

insert into items (name, description) values ('item 1','item 1 description'); 
insert into items (name, description) values ('item 2','item 2 description'); 
insert into items (name, description) values ('item 3','item 3 description'); 
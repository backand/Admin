
/*******************************************************************************
   Chinook Database - Version 1.4
   Script: Chinook_MySql_AutoIncrementPKs.sql
   Description: Creates and populates the Chinook database.
   DB Server: MySql
   Author: Luis Rocha
   License: http://www.codeplex.com/ChinookDatabase/license
********************************************************************************/
use '__DB__Name__';



/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table 'Album'
--

DROP TABLE IF EXISTS 'Album';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'Album' (
  'AlbumId' int(11) NOT NULL AUTO_INCREMENT,
  'Title' varchar(160) CHARACTER SET utf8 NOT NULL,
  'ArtistId' int(11) NOT NULL,
  PRIMARY KEY ('AlbumId'),
  KEY 'IFK_AlbumArtistId' ('ArtistId'),
  CONSTRAINT 'FK_AlbumArtistId' FOREIGN KEY ('ArtistId') REFERENCES 'Artist' ('ArtistId') ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=348 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'Album'
--

LOCK TABLES 'Album' WRITE;
/*!40000 ALTER TABLE 'Album' DISABLE KEYS */;
INSERT INTO 'Album' VALUES (1,'For Those About To Rock We Salute You',1),(2,'Balls to the Wall',2),(3,'Restless and Wild',2),(4,'Let There Be Rock',1),(5,'Big Ones',3),(6,'Jagged Little Pill',4),(7,'Facelift',5),(8,'Warner 25 Anos',6),(9,'Plays Metallica By Four Cellos',7),(10,'Audioslave',8),(11,'Out Of Exile',8),(12,'BackBeat Soundtrack',9),(13,'The Best Of Billy Cobham',10),(14,'Alcohol Fueled Brewtality Live! [Disc 1]',11),(15,'Alcohol Fueled Brewtality Live! [Disc 2]',11),(16,'Black Sabbath',12),(17,'Black Sabbath Vol. 4 (Remaster)',12),(18,'Body Count',13),(19,'Chemical Wedding',14),(20,'The Best Of Buddy Guy - The Millenium Collection',15),(21,'Prenda Minha',16),(22,'Sozinho Remix Ao Vivo',16),(23,'Minha Historia',17),(24,'Afrociberdelia',18),(25,'Da Lama Ao Caos',18),(26,'Acústico MTV [Live]',19),(27,'Cidade Negra - Hits',19),(28,'Na Pista',20),(29,'Axé Bahia 2001',21),(30,'BBC Sessions [Disc 1] [Live]',22),(31,'Bongo Fury',23),(32,'Carnaval 2001',21),(33,'Chill: Brazil (Disc 1)',24),(34,'Chill: Brazil (Disc 2)',6),(35,'Garage Inc. (Disc 1)',50),(36,'Greatest Hits II',51),(37,'Greatest Kiss',52),(38,'Heart of the Night',53),(39,'International Superhits',54),(40,'Into The Light',55),(41,'Meus Momentos',56),(42,'Minha História',57),(43,'MK III The Final Concerts [Disc 1]',58),(44,'Physical Graffiti [Disc 1]',22),(45,'Sambas De Enredo 2001',21),(46,'Supernatural',59),(47,'The Best of Ed Motta',37),(48,'The Essential Miles Davis [Disc 1]',68),(49,'The Essential Miles Davis [Disc 2]',68),(50,'The Final Concerts (Disc 2)',58),(51,'Up An\' Atom',69),(52,'Vinícius De Moraes - Sem Limite',70),(53,'Vozes do MPB',21),(54,'Chronicle, Vol. 1',76),(55,'Chronicle, Vol. 2',76),(56,'Cássia Eller - Coleção Sem Limite [Disc 2]',77),(57,'Cássia Eller - Sem Limite [Disc 1]',77),(58,'Come Taste The Band',58),(59,'Deep Purple In Rock',58),(60,'Fireball',58),(61,'Knocking at Your Back Door: The Best Of Deep Purple in the 80\'s',58),(62,'Machine Head',58),(63,'Purpendicular',58),(64,'Slaves And Masters',58),(65,'Stormbringer',58),(66,'The Battle Rages On',58),(67,'Vault: Def Leppard\'s Greatest Hits',78),(68,'Outbreak',79),(69,'Djavan Ao Vivo - Vol. 02',80),(70,'Djavan Ao Vivo - Vol. 1',80),(71,'Elis Regina-Minha História',41),(72,'The Cream Of Clapton',81),(73,'Unplugged',81),(74,'Album Of The Year',82),(75,'Angel Dust',82),(76,'King For A Day Fool For A Lifetime',82),(77,'The Real Thing',82),(78,'Deixa Entrar',83),(79,'In Your Honor [Disc 1]',84),(80,'In Your Honor [Disc 2]',84),(81,'One By One',84),(82,'The Colour And The Shape',84),(83,'My Way: The Best Of Frank Sinatra [Disc 1]',85),(84,'Roda De Funk',86),(85,'As Canções de Eu Tu Eles',27),(86,'Quanta Gente Veio Ver (Live)',27),(87,'Quanta Gente Veio ver--Bônus De Carnaval',27),(88,'Faceless',87),(89,'American Idiot',54),(90,'Appetite for Destruction',88),(91,'Use Your Illusion I',88),(92,'Use Your Illusion II',88),(93,'Blue Moods',89),(94,'A Matter of Life and Death',90),(95,'A Real Dead One',90),(96,'A Real Live One',90),(97,'Brave New World',90),(98,'Dance Of Death',90),(99,'Fear Of The Dark',90),(100,'Iron Maiden',90),(101,'Killers',90),(102,'Live After Death',90),(103,'Live At Donington 1992 (Disc 1)',90),(104,'Live At Donington 1992 (Disc 2)',90),(105,'No Prayer For The Dying',90),(106,'Piece Of Mind',90),(107,'Powerslave',90),(108,'Rock In Rio [CD1]',90),(109,'Rock In Rio [CD2]',90),(110,'Seventh Son of a Seventh Son',90),(111,'Somewhere in Time',90),(112,'The Number of The Beast',90),(113,'The X Factor',90),(114,'Virtual XI',90),(115,'Sex Machine',91),(116,'Emergency On Planet Earth',92),(117,'Synkronized',92),(118,'The Return Of The Space Cowboy',92),(119,'Get Born',93),(120,'Are You Experienced?',94),(121,'Surfing with the Alien (Remastered)',95),(122,'Jorge Ben Jor 25 Anos',46),(123,'Jota Quest-1995',96),(124,'Cafezinho',97),(125,'Living After Midnight',98),(126,'Unplugged [Live]',52),(127,'BBC Sessions [Disc 2] [Live]',22),(128,'Coda',22),(129,'Houses Of The Holy',22),(130,'In Through The Out Door',22),(131,'IV',22),(132,'Led Zeppelin I',22),(133,'Led Zeppelin II',22),(134,'Led Zeppelin III',22),(135,'Physical Graffiti [Disc 2]',22),(136,'Presence',22),(137,'The Song Remains The Same (Disc 1)',22),(138,'The Song Remains The Same (Disc 2)',22),(139,'A TempestadeTempestade Ou O Livro Dos Dias',99),(140,'Mais Do Mesmo',99),(141,'Greatest Hits',100),(142,'Lulu Santos - RCA 100 Anos De Música - Álbum 01',101),(143,'Lulu Santos - RCA 100 Anos De Música - Álbum 02',101),(144,'Misplaced Childhood',102),(145,'Barulhinho Bom',103),(146,'Seek And Shall Find: More Of The Best (1963-1981)',104),(147,'The Best Of Men At Work',105),(148,'Black Album',50),(149,'Garage Inc. (Disc 2)',50),(150,'Kill \'Em All',50),(151,'Load',50),(152,'Master Of Puppets',50),(153,'ReLoad',50),(154,'Ride The Lightning',50),(155,'St. Anger',50),(156,'...And Justice For All',50),(157,'Miles Ahead',68),(158,'Milton Nascimento Ao Vivo',42),(159,'Minas',42),(160,'Ace Of Spades',106),(161,'Demorou...',108),(162,'Motley Crue Greatest Hits',109),(163,'From The Muddy Banks Of The Wishkah [Live]',110),(164,'Nevermind',110),(165,'Compositores',111),(166,'Olodum',112),(167,'Acústico MTV',113),(168,'Arquivo II',113),(169,'Arquivo Os Paralamas Do Sucesso',113),(170,'Bark at the Moon (Remastered)',114),(171,'Blizzard of Ozz',114),(172,'Diary of a Madman (Remastered)',114),(173,'No More Tears (Remastered)',114),(174,'Tribute',114),(175,'Walking Into Clarksdale',115),(176,'Original Soundtracks 1',116),(177,'The Beast Live',117),(178,'Live On Two Legs [Live]',118),(179,'Pearl Jam',118),(180,'Riot Act',118),(181,'Ten',118),(182,'Vs.',118),(183,'Dark Side Of The Moon',120),(184,'Os Cães Ladram Mas A Caravana Não Pára',121),(185,'Greatest Hits I',51),(186,'News Of The World',51),(187,'Out Of Time',122),(188,'Green',124),(189,'New Adventures In Hi-Fi',124),(190,'The Best Of R.E.M.: The IRS Years',124),(191,'Cesta Básica',125),(192,'Raul Seixas',126),(193,'Blood Sugar Sex Magik',127),(194,'By The Way',127),(195,'Californication',127),(196,'Retrospective I (1974-1980)',128),(197,'Santana - As Years Go By',59),(198,'Santana Live',59),(199,'Maquinarama',130),(200,'O Samba Poconé',130),(201,'Judas 0: B-Sides and Rarities',131),(202,'Rotten Apples: Greatest Hits',131),(203,'A-Sides',132),(204,'Morning Dance',53),(205,'In Step',133),(206,'Core',134),(207,'Mezmerize',135),(208,'[1997] Black Light Syndrome',136),(209,'Live [Disc 1]',137),(210,'Live [Disc 2]',137),(211,'The Singles',138),(212,'Beyond Good And Evil',139),(213,'Pure Cult: The Best Of The Cult (For Rockers, Ravers, Lovers & Sinners) [UK]',139),(214,'The Doors',140),(215,'The Police Greatest Hits',141),(216,'Hot Rocks, 1964-1971 (Disc 1)',142),(217,'No Security',142),(218,'Voodoo Lounge',142),(219,'Tangents',143),(220,'Transmission',143),(221,'My Generation - The Very Best Of The Who',144),(222,'Serie Sem Limite (Disc 1)',145),(223,'Serie Sem Limite (Disc 2)',145),(224,'Acústico',146),(225,'Volume Dois',146),(226,'Battlestar Galactica: The Story So Far',147),(227,'Battlestar Galactica, Season 3',147),(228,'Heroes, Season 1',148),(229,'Lost, Season 3',149),(230,'Lost, Season 1',149),(231,'Lost, Season 2',149),(232,'Achtung Baby',150),(233,'All That You Can\'t Leave Behind',150),(234,'B-Sides 1980-1990',150),(235,'How To Dismantle An Atomic Bomb',150),(236,'Pop',150),(237,'Rattle And Hum',150),(238,'The Best Of 1980-1990',150),(239,'War',150),(240,'Zooropa',150),(241,'UB40 The Best Of - Volume Two [UK]',151),(242,'Diver Down',152),(243,'The Best Of Van Halen, Vol. I',152),(244,'Van Halen',152),(245,'Van Halen III',152),(246,'Contraband',153),(247,'Vinicius De Moraes',72),(248,'Ao Vivo [IMPORT]',155),(249,'The Office, Season 1',156),(250,'The Office, Season 2',156),(251,'The Office, Season 3',156),(252,'Un-Led-Ed',157),(253,'Battlestar Galactica (Classic), Season 1',158),(254,'Aquaman',159),(255,'Instant Karma: The Amnesty International Campaign to Save Darfur',150),(256,'Speak of the Devil',114),(257,'20th Century Masters - The Millennium Collection: The Best of Scorpions',179),(258,'House of Pain',180),(259,'Radio Brasil (O Som da Jovem Vanguarda) - Seleccao de Henrique Amaro',36),(260,'Cake: B-Sides and Rarities',196),(261,'LOST, Season 4',149),(262,'Quiet Songs',197),(263,'Muso Ko',198),(264,'Realize',199),(265,'Every Kind of Light',200),(266,'Duos II',201),(267,'Worlds',202),(268,'The Best of Beethoven',203),(269,'Temple of the Dog',204),(270,'Carry On',205),(271,'Revelations',8),(272,'Adorate Deum: Gregorian Chant from the Proper of the Mass',206),(273,'Allegri: Miserere',207),(274,'Pachelbel: Canon & Gigue',208),(275,'Vivaldi: The Four Seasons',209),(276,'Bach: Violin Concertos',210),(277,'Bach: Goldberg Variations',211),(278,'Bach: The Cello Suites',212),(279,'Handel: The Messiah (Highlights)',213),(280,'The World of Classical Favourites',214),(281,'Sir Neville Marriner: A Celebration',215),(282,'Mozart: Wind Concertos',216),(283,'Haydn: Symphonies 99 - 104',217),(284,'Beethoven: Symhonies Nos. 5 & 6',218),(285,'A Soprano Inspired',219),(286,'Great Opera Choruses',220),(287,'Wagner: Favourite Overtures',221),(288,'Fauré: Requiem, Ravel: Pavane & Others',222),(289,'Tchaikovsky: The Nutcracker',223),(290,'The Last Night of the Proms',224),(291,'Puccini: Madama Butterfly - Highlights',225),(292,'Holst: The Planets, Op. 32 & Vaughan Williams: Fantasies',226),(293,'Pavarotti\'s Opera Made Easy',227),(294,'Great Performances - Barber\'s Adagio and Other Romantic Favorites for Strings',228),(295,'Carmina Burana',229),(296,'A Copland Celebration, Vol. I',230),(297,'Bach: Toccata & Fugue in D Minor',231),(298,'Prokofiev: Symphony No.1',232),(299,'Scheherazade',233),(300,'Bach: The Brandenburg Concertos',234),(301,'Chopin: Piano Concertos Nos. 1 & 2',235),(302,'Mascagni: Cavalleria Rusticana',236),(303,'Sibelius: Finlandia',237),(304,'Beethoven Piano Sonatas: Moonlight & Pastorale',238),(305,'Great Recordings of the Century - Mahler: Das Lied von der Erde',240),(306,'Elgar: Cello Concerto & Vaughan Williams: Fantasias',241),(307,'Adams, John: The Chairman Dances',242),(308,'Tchaikovsky: 1812 Festival Overture, Op.49, Capriccio Italien & Beethoven: Wellington\'s Victory',243),(309,'Palestrina: Missa Papae Marcelli & Allegri: Miserere',244),(310,'Prokofiev: Romeo & Juliet',245),(311,'Strauss: Waltzes',226),(312,'Berlioz: Symphonie Fantastique',245),(313,'Bizet: Carmen Highlights',246),(314,'English Renaissance',247),(315,'Handel: Music for the Royal Fireworks (Original Version 1749)',208),(316,'Grieg: Peer Gynt Suites & Sibelius: Pelléas et Mélisande',248),(317,'Mozart Gala: Famous Arias',249),(318,'SCRIABIN: Vers la flamme',250),(319,'Armada: Music from the Courts of England and Spain',251),(320,'Mozart: Symphonies Nos. 40 & 41',248),(321,'Back to Black',252),(322,'Frank',252),(323,'Carried to Dust (Bonus Track Version)',253),(324,'Beethoven: Symphony No. 6 \'Pastoral\' Etc.',254),(325,'Bartok: Violin & Viola Concertos',255),(326,'Mendelssohn: A Midsummer Night\'s Dream',256),(327,'Bach: Orchestral Suites Nos. 1 - 4',257),(328,'Charpentier: Divertissements, Airs & Concerts',258),(329,'South American Getaway',259),(330,'Górecki: Symphony No. 3',260),(331,'Purcell: The Fairy Queen',261),(332,'The Ultimate Relexation Album',262),(333,'Purcell: Music for the Queen Mary',263),(334,'Weill: The Seven Deadly Sins',264),(335,'J.S. Bach: Chaconne, Suite in E Minor, Partita in E Major & Prelude, Fugue and Allegro',265),(336,'Prokofiev: Symphony No.5 & Stravinksy: Le Sacre Du Printemps',248),(337,'Szymanowski: Piano Works, Vol. 1',266),(338,'Nielsen: The Six Symphonies',267),(339,'Great Recordings of the Century: Paganini\'s 24 Caprices',268),(340,'Liszt - 12 Études D\'Execution Transcendante',269),(341,'Great Recordings of the Century - Shubert: Schwanengesang, 4 Lieder',270),(342,'Locatelli: Concertos for Violin, Strings and Continuo, Vol. 3',271),(343,'Respighi:Pines of Rome',226),(344,'Schubert: The Late String Quartets & String Quintet (3 CD\'s)',272),(345,'Monteverdi: L\'Orfeo',273),(346,'Mozart: Chamber Music',274),(347,'Koyaanisqatsi (Soundtrack from the Motion Picture)',275);
/*!40000 ALTER TABLE 'Album' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'Artist'
--

DROP TABLE IF EXISTS 'Artist';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'Artist' (
  'ArtistId' int(11) NOT NULL AUTO_INCREMENT,
  'Name' varchar(120) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY ('ArtistId')
) ENGINE=InnoDB AUTO_INCREMENT=276 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'Artist'
--

LOCK TABLES 'Artist' WRITE;
/*!40000 ALTER TABLE 'Artist' DISABLE KEYS */;
INSERT INTO 'Artist' VALUES (1,'AC/DC'),(2,'Accept'),(3,'Aerosmith'),(4,'Alanis Morissette'),(5,'Alice In Chains'),(6,'Antônio Carlos Jobim'),(7,'Apocalyptica'),(8,'Audioslave'),(9,'BackBeat'),(10,'Billy Cobham'),(11,'Black Label Society'),(12,'Black Sabbath'),(13,'Body Count'),(14,'Bruce Dickinson'),(15,'Buddy Guy'),(16,'Caetano Veloso'),(17,'Chico Buarque'),(18,'Chico Science & Nação Zumbi'),(19,'Cidade Negra'),(20,'Cláudio Zoli'),(21,'Various Artists'),(22,'Led Zeppelin'),(23,'Frank Zappa & Captain Beefheart'),(24,'Marcos Valle'),(25,'Milton Nascimento & Bebeto'),(26,'Azymuth'),(27,'Gilberto Gil'),(28,'João Gilberto'),(29,'Bebel Gilberto'),(30,'Jorge Vercilo'),(31,'Baby Consuelo'),(32,'Ney Matogrosso'),(33,'Luiz Melodia'),(34,'Nando Reis'),(35,'Pedro Luís & A Parede'),(36,'O Rappa'),(37,'Ed Motta'),(38,'Banda Black Rio'),(39,'Fernanda Porto'),(40,'Os Cariocas'),(41,'Elis Regina'),(42,'Milton Nascimento'),(43,'A Cor Do Som'),(44,'Kid Abelha'),(45,'Sandra De Sá'),(46,'Jorge Ben'),(47,'Hermeto Pascoal'),(48,'Barão Vermelho'),(49,'Edson, DJ Marky & DJ Patife Featuring Fernanda Porto'),(50,'Metallica'),(51,'Queen'),(52,'Kiss'),(53,'Spyro Gyra'),(54,'Green Day'),(55,'David Coverdale'),(56,'Gonzaguinha'),(57,'Os Mutantes'),(58,'Deep Purple'),(59,'Santana'),(60,'Santana Feat. Dave Matthews'),(61,'Santana Feat. Everlast'),(62,'Santana Feat. Rob Thomas'),(63,'Santana Feat. Lauryn Hill & Cee-Lo'),(64,'Santana Feat. The Project G&B'),(65,'Santana Feat. Maná'),(66,'Santana Feat. Eagle-Eye Cherry'),(67,'Santana Feat. Eric Clapton'),(68,'Miles Davis'),(69,'Gene Krupa'),(70,'Toquinho & Vinícius'),(71,'Vinícius De Moraes & Baden Powell'),(72,'Vinícius De Moraes'),(73,'Vinícius E Qurteto Em Cy'),(74,'Vinícius E Odette Lara'),(75,'Vinicius, Toquinho & Quarteto Em Cy'),(76,'Creedence Clearwater Revival'),(77,'Cássia Eller'),(78,'Def Leppard'),(79,'Dennis Chambers'),(80,'Djavan'),(81,'Eric Clapton'),(82,'Faith No More'),(83,'Falamansa'),(84,'Foo Fighters'),(85,'Frank Sinatra'),(86,'Funk Como Le Gusta'),(87,'Godsmack'),(88,'Guns N\' Roses'),(89,'Incognito'),(90,'Iron Maiden'),(91,'James Brown'),(92,'Jamiroquai'),(93,'JET'),(94,'Jimi Hendrix'),(95,'Joe Satriani'),(96,'Jota Quest'),(97,'João Suplicy'),(98,'Judas Priest'),(99,'Legião Urbana'),(100,'Lenny Kravitz'),(101,'Lulu Santos'),(102,'Marillion'),(103,'Marisa Monte'),(104,'Marvin Gaye'),(105,'Men At Work'),(106,'Motörhead'),(107,'Motörhead & Girlschool'),(108,'Mônica Marianno'),(109,'Mötley Crüe'),(110,'Nirvana'),(111,'O Terço'),(112,'Olodum'),(113,'Os Paralamas Do Sucesso'),(114,'Ozzy Osbourne'),(115,'Page & Plant'),(116,'Passengers'),(117,'Paul D\'Ianno'),(118,'Pearl Jam'),(119,'Peter Tosh'),(120,'Pink Floyd'),(121,'Planet Hemp'),(122,'R.E.M. Feat. Kate Pearson'),(123,'R.E.M. Feat. KRS-One'),(124,'R.E.M.'),(125,'Raimundos'),(126,'Raul Seixas'),(127,'Red Hot Chili Peppers'),(128,'Rush'),(129,'Simply Red'),(130,'Skank'),(131,'Smashing Pumpkins'),(132,'Soundgarden'),(133,'Stevie Ray Vaughan & Double Trouble'),(134,'Stone Temple Pilots'),(135,'System Of A Down'),(136,'Terry Bozzio, Tony Levin & Steve Stevens'),(137,'The Black Crowes'),(138,'The Clash'),(139,'The Cult'),(140,'The Doors'),(141,'The Police'),(142,'The Rolling Stones'),(143,'The Tea Party'),(144,'The Who'),(145,'Tim Maia'),(146,'Titãs'),(147,'Battlestar Galactica'),(148,'Heroes'),(149,'Lost'),(150,'U2'),(151,'UB40'),(152,'Van Halen'),(153,'Velvet Revolver'),(154,'Whitesnake'),(155,'Zeca Pagodinho'),(156,'The Office'),(157,'Dread Zeppelin'),(158,'Battlestar Galactica (Classic)'),(159,'Aquaman'),(160,'Christina Aguilera featuring BigElf'),(161,'Aerosmith & Sierra Leone\'s Refugee Allstars'),(162,'Los Lonely Boys'),(163,'Corinne Bailey Rae'),(164,'Dhani Harrison & Jakob Dylan'),(165,'Jackson Browne'),(166,'Avril Lavigne'),(167,'Big & Rich'),(168,'Youssou N\'Dour'),(169,'Black Eyed Peas'),(170,'Jack Johnson'),(171,'Ben Harper'),(172,'Snow Patrol'),(173,'Matisyahu'),(174,'The Postal Service'),(175,'Jaguares'),(176,'The Flaming Lips'),(177,'Jack\'s Mannequin & Mick Fleetwood'),(178,'Regina Spektor'),(179,'Scorpions'),(180,'House Of Pain'),(181,'Xis'),(182,'Nega Gizza'),(183,'Gustavo & Andres Veiga & Salazar'),(184,'Rodox'),(185,'Charlie Brown Jr.'),(186,'Pedro Luís E A Parede'),(187,'Los Hermanos'),(188,'Mundo Livre S/A'),(189,'Otto'),(190,'Instituto'),(191,'Nação Zumbi'),(192,'DJ Dolores & Orchestra Santa Massa'),(193,'Seu Jorge'),(194,'Sabotage E Instituto'),(195,'Stereo Maracana'),(196,'Cake'),(197,'Aisha Duo'),(198,'Habib Koité and Bamada'),(199,'Karsh Kale'),(200,'The Posies'),(201,'Luciana Souza/Romero Lubambo'),(202,'Aaron Goldberg'),(203,'Nicolaus Esterhazy Sinfonia'),(204,'Temple of the Dog'),(205,'Chris Cornell'),(206,'Alberto Turco & Nova Schola Gregoriana'),(207,'Richard Marlow & The Choir of Trinity College, Cambridge'),(208,'English Concert & Trevor Pinnock'),(209,'Anne-Sophie Mutter, Herbert Von Karajan & Wiener Philharmoniker'),(210,'Hilary Hahn, Jeffrey Kahane, Los Angeles Chamber Orchestra & Margaret Batjer'),(211,'Wilhelm Kempff'),(212,'Yo-Yo Ma'),(213,'Scholars Baroque Ensemble'),(214,'Academy of St. Martin in the Fields & Sir Neville Marriner'),(215,'Academy of St. Martin in the Fields Chamber Ensemble & Sir Neville Marriner'),(216,'Berliner Philharmoniker, Claudio Abbado & Sabine Meyer'),(217,'Royal Philharmonic Orchestra & Sir Thomas Beecham'),(218,'Orchestre Révolutionnaire et Romantique & John Eliot Gardiner'),(219,'Britten Sinfonia, Ivor Bolton & Lesley Garrett'),(220,'Chicago Symphony Chorus, Chicago Symphony Orchestra & Sir Georg Solti'),(221,'Sir Georg Solti & Wiener Philharmoniker'),(222,'Academy of St. Martin in the Fields, John Birch, Sir Neville Marriner & Sylvia McNair'),(223,'London Symphony Orchestra & Sir Charles Mackerras'),(224,'Barry Wordsworth & BBC Concert Orchestra'),(225,'Herbert Von Karajan, Mirella Freni & Wiener Philharmoniker'),(226,'Eugene Ormandy'),(227,'Luciano Pavarotti'),(228,'Leonard Bernstein & New York Philharmonic'),(229,'Boston Symphony Orchestra & Seiji Ozawa'),(230,'Aaron Copland & London Symphony Orchestra'),(231,'Ton Koopman'),(232,'Sergei Prokofiev & Yuri Temirkanov'),(233,'Chicago Symphony Orchestra & Fritz Reiner'),(234,'Orchestra of The Age of Enlightenment'),(235,'Emanuel Ax, Eugene Ormandy & Philadelphia Orchestra'),(236,'James Levine'),(237,'Berliner Philharmoniker & Hans Rosbaud'),(238,'Maurizio Pollini'),(239,'Academy of St. Martin in the Fields, Sir Neville Marriner & William Bennett'),(240,'Gustav Mahler'),(241,'Felix Schmidt, London Symphony Orchestra & Rafael Frühbeck de Burgos'),(242,'Edo de Waart & San Francisco Symphony'),(243,'Antal Doráti & London Symphony Orchestra'),(244,'Choir Of Westminster Abbey & Simon Preston'),(245,'Michael Tilson Thomas & San Francisco Symphony'),(246,'Chor der Wiener Staatsoper, Herbert Von Karajan & Wiener Philharmoniker'),(247,'The King\'s Singers'),(248,'Berliner Philharmoniker & Herbert Von Karajan'),(249,'Sir Georg Solti, Sumi Jo & Wiener Philharmoniker'),(250,'Christopher O\'Riley'),(251,'Fretwork'),(252,'Amy Winehouse'),(253,'Calexico'),(254,'Otto Klemperer & Philharmonia Orchestra'),(255,'Yehudi Menuhin'),(256,'Philharmonia Orchestra & Sir Neville Marriner'),(257,'Academy of St. Martin in the Fields, Sir Neville Marriner & Thurston Dart'),(258,'Les Arts Florissants & William Christie'),(259,'The 12 Cellists of The Berlin Philharmonic'),(260,'Adrian Leaper & Doreen de Feis'),(261,'Roger Norrington, London Classical Players'),(262,'Charles Dutoit & L\'Orchestre Symphonique de Montréal'),(263,'Equale Brass Ensemble, John Eliot Gardiner & Munich Monteverdi Orchestra and Choir'),(264,'Kent Nagano and Orchestre de l\'Opéra de Lyon'),(265,'Julian Bream'),(266,'Martin Roscoe'),(267,'Göteborgs Symfoniker & Neeme Järvi'),(268,'Itzhak Perlman'),(269,'Michele Campanella'),(270,'Gerald Moore'),(271,'Mela Tenenbaum, Pro Musica Prague & Richard Kapp'),(272,'Emerson String Quartet'),(273,'C. Monteverdi, Nigel Rogers - Chiaroscuro; London Baroque; London Cornett & Sackbu'),(274,'Nash Ensemble'),(275,'Philip Glass Ensemble');
/*!40000 ALTER TABLE 'Artist' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'Customer'
--

DROP TABLE IF EXISTS 'Customer';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'Customer' (
  'CustomerId' int(11) NOT NULL AUTO_INCREMENT,
  'FirstName' varchar(40) CHARACTER SET utf8 NOT NULL,
  'LastName' varchar(20) CHARACTER SET utf8 NOT NULL,
  'Company' varchar(80) CHARACTER SET utf8 DEFAULT NULL,
  'Address' varchar(70) CHARACTER SET utf8 DEFAULT NULL,
  'City' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'State' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'Country' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'PostalCode' varchar(10) CHARACTER SET utf8 DEFAULT NULL,
  'Phone' varchar(24) CHARACTER SET utf8 DEFAULT NULL,
  'Fax' varchar(24) CHARACTER SET utf8 DEFAULT NULL,
  'Email' varchar(60) CHARACTER SET utf8 NOT NULL,
  'SupportRepId' int(11) DEFAULT NULL,
  PRIMARY KEY ('CustomerId'),
  KEY 'IFK_CustomerSupportRepId' ('SupportRepId'),
  CONSTRAINT 'FK_CustomerSupportRepId' FOREIGN KEY ('SupportRepId') REFERENCES 'Employee' ('EmployeeId') ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=60 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'Customer'
--

LOCK TABLES 'Customer' WRITE;
/*!40000 ALTER TABLE 'Customer' DISABLE KEYS */;
INSERT INTO 'Customer' VALUES (1,'Luís','Gonçalves','Embraer - Empresa Brasileira de Aeronáutica S.A.','Av. Brigadeiro Faria Lima, 2170','São José dos Campos','SP','Brazil','12227-000','+55 (12) 3923-5555','+55 (12) 3923-5566','luisg@embraer.com.br',3),(2,'Leonie','Köhler',NULL,'Theodor-Heuss-Straße 34','Stuttgart',NULL,'Germany','70174','+49 0711 2842222',NULL,'leonekohler@surfeu.de',5),(3,'François','Tremblay',NULL,'1498 rue Bélanger','Montréal','QC','Canada','H2G 1A7','+1 (514) 721-4711',NULL,'ftremblay@gmail.com',3),(4,'Bjørn','Hansen',NULL,'Ullevålsveien 14','Oslo',NULL,'Norway','0171','+47 22 44 22 22',NULL,'bjorn.hansen@yahoo.no',4),(5,'František','Wichterlová','JetBrains s.r.o.','Klanova 9/506','Prague',NULL,'Czech Republic','14700','+420 2 4172 5555','+420 2 4172 5555','frantisekw@jetbrains.com',4),(6,'Helena','Holý',NULL,'Rilská 3174/6','Prague',NULL,'Czech Republic','14300','+420 2 4177 0449',NULL,'hholy@gmail.com',5),(7,'Astrid','Gruber',NULL,'Rotenturmstraße 4, 1010 Innere Stadt','Vienne',NULL,'Austria','1010','+43 01 5134505',NULL,'astrid.gruber@apple.at',5),(8,'Daan','Peeters',NULL,'Grétrystraat 63','Brussels',NULL,'Belgium','1000','+32 02 219 03 03',NULL,'daan_peeters@apple.be',4),(9,'Kara','Nielsen',NULL,'Sønder Boulevard 51','Copenhagen',NULL,'Denmark','1720','+453 3331 9991',NULL,'kara.nielsen@jubii.dk',4),(10,'Eduardo','Martins','Woodstock Discos','Rua Dr. Falcão Filho, 155','São Paulo','SP','Brazil','01007-010','+55 (11) 3033-5446','+55 (11) 3033-4564','eduardo@woodstock.com.br',4),(11,'Alexandre','Rocha','Banco do Brasil S.A.','Av. Paulista, 2022','São Paulo','SP','Brazil','01310-200','+55 (11) 3055-3278','+55 (11) 3055-8131','alero@uol.com.br',5),(12,'Roberto','Almeida','Riotur','Praça Pio X, 119','Rio de Janeiro','RJ','Brazil','20040-020','+55 (21) 2271-7000','+55 (21) 2271-7070','roberto.almeida@riotur.gov.br',3),(13,'Fernanda','Ramos',NULL,'Qe 7 Bloco G','Brasília','DF','Brazil','71020-677','+55 (61) 3363-5547','+55 (61) 3363-7855','fernadaramos4@uol.com.br',4),(14,'Mark','Philips','Telus','8210 111 ST NW','Edmonton','AB','Canada','T6G 2C7','+1 (780) 434-4554','+1 (780) 434-5565','mphilips12@shaw.ca',5),(15,'Jennifer','Peterson','Rogers Canada','700 W Pender Street','Vancouver','BC','Canada','V6C 1G8','+1 (604) 688-2255','+1 (604) 688-8756','jenniferp@rogers.ca',3),(16,'Frank','Harris','Google Inc.','1600 Amphitheatre Parkway','Mountain View','CA','USA','94043-1351','+1 (650) 253-0000','+1 (650) 253-0000','fharris@google.com',4),(17,'Jack','Smith','Microsoft Corporation','1 Microsoft Way','Redmond','WA','USA','98052-8300','+1 (425) 882-8080','+1 (425) 882-8081','jacksmith@microsoft.com',5),(18,'Michelle','Brooks',NULL,'627 Broadway','New York','NY','USA','10012-2612','+1 (212) 221-3546','+1 (212) 221-4679','michelleb@aol.com',3),(19,'Tim','Goyer','Apple Inc.','1 Infinite Loop','Cupertino','CA','USA','95014','+1 (408) 996-1010','+1 (408) 996-1011','tgoyer@apple.com',3),(20,'Dan','Miller',NULL,'541 Del Medio Avenue','Mountain View','CA','USA','94040-111','+1 (650) 644-3358',NULL,'dmiller@comcast.com',4),(21,'Kathy','Chase',NULL,'801 W 4th Street','Reno','NV','USA','89503','+1 (775) 223-7665',NULL,'kachase@hotmail.com',5),(22,'Heather','Leacock',NULL,'120 S Orange Ave','Orlando','FL','USA','32801','+1 (407) 999-7788',NULL,'hleacock@gmail.com',4),(23,'John','Gordon',NULL,'69 Salem Street','Boston','MA','USA','2113','+1 (617) 522-1333',NULL,'johngordon22@yahoo.com',4),(24,'Frank','Ralston',NULL,'162 E Superior Street','Chicago','IL','USA','60611','+1 (312) 332-3232',NULL,'fralston@gmail.com',3),(25,'Victor','Stevens',NULL,'319 N. Frances Street','Madison','WI','USA','53703','+1 (608) 257-0597',NULL,'vstevens@yahoo.com',5),(26,'Richard','Cunningham',NULL,'2211 W Berry Street','Fort Worth','TX','USA','76110','+1 (817) 924-7272',NULL,'ricunningham@hotmail.com',4),(27,'Patrick','Gray',NULL,'1033 N Park Ave','Tucson','AZ','USA','85719','+1 (520) 622-4200',NULL,'patrick.gray@aol.com',4),(28,'Julia','Barnett',NULL,'302 S 700 E','Salt Lake City','UT','USA','84102','+1 (801) 531-7272',NULL,'jubarnett@gmail.com',5),(29,'Robert','Brown',NULL,'796 Dundas Street West','Toronto','ON','Canada','M6J 1V1','+1 (416) 363-8888',NULL,'robbrown@shaw.ca',3),(30,'Edward','Francis',NULL,'230 Elgin Street','Ottawa','ON','Canada','K2P 1L7','+1 (613) 234-3322',NULL,'edfrancis@yachoo.ca',3),(31,'Martha','Silk',NULL,'194A Chain Lake Drive','Halifax','NS','Canada','B3S 1C5','+1 (902) 450-0450',NULL,'marthasilk@gmail.com',5),(32,'Aaron','Mitchell',NULL,'696 Osborne Street','Winnipeg','MB','Canada','R3L 2B9','+1 (204) 452-6452',NULL,'aaronmitchell@yahoo.ca',4),(33,'Ellie','Sullivan',NULL,'5112 48 Street','Yellowknife','NT','Canada','X1A 1N6','+1 (867) 920-2233',NULL,'ellie.sullivan@shaw.ca',3),(34,'João','Fernandes',NULL,'Rua da Assunção 53','Lisbon',NULL,'Portugal',NULL,'+351 (213) 466-111',NULL,'jfernandes@yahoo.pt',4),(35,'Madalena','Sampaio',NULL,'Rua dos Campeões Europeus de Viena, 4350','Porto',NULL,'Portugal',NULL,'+351 (225) 022-448',NULL,'masampaio@sapo.pt',4),(36,'Hannah','Schneider',NULL,'Tauentzienstraße 8','Berlin',NULL,'Germany','10789','+49 030 26550280',NULL,'hannah.schneider@yahoo.de',5),(37,'Fynn','Zimmermann',NULL,'Berger Straße 10','Frankfurt',NULL,'Germany','60316','+49 069 40598889',NULL,'fzimmermann@yahoo.de',3),(38,'Niklas','Schröder',NULL,'Barbarossastraße 19','Berlin',NULL,'Germany','10779','+49 030 2141444',NULL,'nschroder@surfeu.de',3),(39,'Camille','Bernard',NULL,'4, Rue Milton','Paris',NULL,'France','75009','+33 01 49 70 65 65',NULL,'camille.bernard@yahoo.fr',4),(40,'Dominique','Lefebvre',NULL,'8, Rue Hanovre','Paris',NULL,'France','75002','+33 01 47 42 71 71',NULL,'dominiquelefebvre@gmail.com',4),(41,'Marc','Dubois',NULL,'11, Place Bellecour','Lyon',NULL,'France','69002','+33 04 78 30 30 30',NULL,'marc.dubois@hotmail.com',5),(42,'Wyatt','Girard',NULL,'9, Place Louis Barthou','Bordeaux',NULL,'France','33000','+33 05 56 96 96 96',NULL,'wyatt.girard@yahoo.fr',3),(43,'Isabelle','Mercier',NULL,'68, Rue Jouvence','Dijon',NULL,'France','21000','+33 03 80 73 66 99',NULL,'isabelle_mercier@apple.fr',3),(44,'Terhi','Hämäläinen',NULL,'Porthaninkatu 9','Helsinki',NULL,'Finland','00530','+358 09 870 2000',NULL,'terhi.hamalainen@apple.fi',3),(45,'Ladislav','Kovács',NULL,'Erzsébet krt. 58.','Budapest',NULL,'Hungary','H-1073',NULL,NULL,'ladislav_kovacs@apple.hu',3),(46,'Hugh','O\'Reilly',NULL,'3 Chatham Street','Dublin','Dublin','Ireland',NULL,'+353 01 6792424',NULL,'hughoreilly@apple.ie',3),(47,'Lucas','Mancini',NULL,'Via Degli Scipioni, 43','Rome','RM','Italy','00192','+39 06 39733434',NULL,'lucas.mancini@yahoo.it',5),(48,'Johannes','Van der Berg',NULL,'Lijnbaansgracht 120bg','Amsterdam','VV','Netherlands','1016','+31 020 6223130',NULL,'johavanderberg@yahoo.nl',5),(49,'Stanisław','Wójcik',NULL,'Ordynacka 10','Warsaw',NULL,'Poland','00-358','+48 22 828 37 39',NULL,'stanisław.wójcik@wp.pl',4),(50,'Enrique','Muñoz',NULL,'C/ San Bernardo 85','Madrid',NULL,'Spain','28015','+34 914 454 454',NULL,'enrique_munoz@yahoo.es',5),(51,'Joakim','Johansson',NULL,'Celsiusg. 9','Stockholm',NULL,'Sweden','11230','+46 08-651 52 52',NULL,'joakim.johansson@yahoo.se',5),(52,'Emma','Jones',NULL,'202 Hoxton Street','London',NULL,'United Kingdom','N1 5LH','+44 020 7707 0707',NULL,'emma_jones@hotmail.com',3),(53,'Phil','Hughes',NULL,'113 Lupus St','London',NULL,'United Kingdom','SW1V 3EN','+44 020 7976 5722',NULL,'phil.hughes@gmail.com',3),(54,'Steve','Murray',NULL,'110 Raeburn Pl','Edinburgh ',NULL,'United Kingdom','EH4 1HH','+44 0131 315 3300',NULL,'steve.murray@yahoo.uk',5),(55,'Mark','Taylor',NULL,'421 Bourke Street','Sidney','NSW','Australia','2010','+61 (02) 9332 3633',NULL,'mark.taylor@yahoo.au',4),(56,'Diego','Gutiérrez',NULL,'307 Macacha Güemes','Buenos Aires',NULL,'Argentina','1106','+54 (0)11 4311 4333',NULL,'diego.gutierrez@yahoo.ar',4),(57,'Luis','Rojas',NULL,'Calle Lira, 198','Santiago',NULL,'Chile',NULL,'+56 (0)2 635 4444',NULL,'luisrojas@yahoo.cl',5),(58,'Manoj','Pareek',NULL,'12,Community Centre','Delhi',NULL,'India','110017','+91 0124 39883988',NULL,'manoj.pareek@rediff.com',3),(59,'Puja','Srivastava',NULL,'3,Raj Bhavan Road','Bangalore',NULL,'India','560001','+91 080 22289999',NULL,'puja_srivastava@yahoo.in',3);
/*!40000 ALTER TABLE 'Customer' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'Employee'
--

DROP TABLE IF EXISTS 'Employee';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'Employee' (
  'EmployeeId' int(11) NOT NULL AUTO_INCREMENT,
  'LastName' varchar(20) CHARACTER SET utf8 NOT NULL,
  'FirstName' varchar(20) CHARACTER SET utf8 NOT NULL,
  'Title' varchar(30) CHARACTER SET utf8 DEFAULT NULL,
  'ReportsTo' int(11) DEFAULT NULL,
  'BirthDate' datetime DEFAULT NULL,
  'HireDate' datetime DEFAULT NULL,
  'Address' varchar(70) CHARACTER SET utf8 DEFAULT NULL,
  'City' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'State' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'Country' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'PostalCode' varchar(10) CHARACTER SET utf8 DEFAULT NULL,
  'Phone' varchar(24) CHARACTER SET utf8 DEFAULT NULL,
  'Fax' varchar(24) CHARACTER SET utf8 DEFAULT NULL,
  'Email' varchar(60) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY ('EmployeeId'),
  KEY 'IFK_EmployeeReportsTo' ('ReportsTo'),
  CONSTRAINT 'FK_EmployeeReportsTo' FOREIGN KEY ('ReportsTo') REFERENCES 'Employee' ('EmployeeId') ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'Employee'
--

LOCK TABLES 'Employee' WRITE;
/*!40000 ALTER TABLE 'Employee' DISABLE KEYS */;
INSERT INTO 'Employee' VALUES (1,'Adams','Andrew','General Manager',NULL,'1962-02-18 00:00:00','2002-08-14 00:00:00','11120 Jasper Ave NW','Edmonton','AB','Canada','T5K 2N1','+1 (780) 428-9482','+1 (780) 428-3457','andrew@chinookcorp.com'),(2,'Edwards','Nancy','Sales Manager',1,'1958-12-08 00:00:00','2002-05-01 00:00:00','825 8 Ave SW','Calgary','AB','Canada','T2P 2T3','+1 (403) 262-3443','+1 (403) 262-3322','nancy@chinookcorp.com'),(3,'Peacock','Jane','Sales Support Agent',2,'1973-08-29 00:00:00','2002-04-01 00:00:00','1111 6 Ave SW','Calgary','AB','Canada','T2P 5M5','+1 (403) 262-3443','+1 (403) 262-6712','jane@chinookcorp.com'),(4,'Park','Margaret','Sales Support Agent',2,'1947-09-19 00:00:00','2003-05-03 00:00:00','683 10 Street SW','Calgary','AB','Canada','T2P 5G3','+1 (403) 263-4423','+1 (403) 263-4289','margaret@chinookcorp.com'),(5,'Johnson','Steve','Sales Support Agent',2,'1965-03-03 00:00:00','2003-10-17 00:00:00','7727B 41 Ave','Calgary','AB','Canada','T3B 1Y7','1 (780) 836-9987','1 (780) 836-9543','steve@chinookcorp.com'),(6,'Mitchell','Michael','IT Manager',1,'1973-07-01 00:00:00','2003-10-17 00:00:00','5827 Bowness Road NW','Calgary','AB','Canada','T3B 0C5','+1 (403) 246-9887','+1 (403) 246-9899','michael@chinookcorp.com'),(7,'King','Robert','IT Staff',6,'1970-05-29 00:00:00','2004-01-02 00:00:00','590 Columbia Boulevard West','Lethbridge','AB','Canada','T1K 5N8','+1 (403) 456-9986','+1 (403) 456-8485','robert@chinookcorp.com'),(8,'Callahan','Laura','IT Staff',6,'1968-01-09 00:00:00','2004-03-04 00:00:00','923 7 ST NW','Lethbridge','AB','Canada','T1H 1Y8','+1 (403) 467-3351','+1 (403) 467-8772','laura@chinookcorp.com');
/*!40000 ALTER TABLE 'Employee' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'Genre'
--

DROP TABLE IF EXISTS 'Genre';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'Genre' (
  'GenreId' int(11) NOT NULL AUTO_INCREMENT,
  'Name' varchar(120) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY ('GenreId')
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'Genre'
--

LOCK TABLES 'Genre' WRITE;
/*!40000 ALTER TABLE 'Genre' DISABLE KEYS */;
INSERT INTO 'Genre' VALUES (1,'Rock'),(2,'Jazz'),(3,'Metal'),(4,'Alternative & Punk'),(5,'Rock And Roll'),(6,'Blues'),(7,'Latin'),(8,'Reggae'),(9,'Pop'),(10,'Soundtrack'),(11,'Bossa Nova'),(12,'Easy Listening'),(13,'Heavy Metal'),(14,'R&B/Soul'),(15,'Electronica/Dance'),(16,'World'),(17,'Hip Hop/Rap'),(18,'Science Fiction'),(19,'TV Shows'),(20,'Sci Fi & Fantasy'),(21,'Drama'),(22,'Comedy'),(23,'Alternative'),(24,'Classical'),(25,'Opera');
/*!40000 ALTER TABLE 'Genre' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'Invoice'
--

DROP TABLE IF EXISTS 'Invoice';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'Invoice' (
  'InvoiceId' int(11) NOT NULL AUTO_INCREMENT,
  'CustomerId' int(11) NOT NULL,
  'InvoiceDate' datetime NOT NULL,
  'BillingAddress' varchar(70) CHARACTER SET utf8 DEFAULT NULL,
  'BillingCity' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'BillingState' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'BillingCountry' varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  'BillingPostalCode' varchar(10) CHARACTER SET utf8 DEFAULT NULL,
  'Total' decimal(10,2) NOT NULL,
  PRIMARY KEY ('InvoiceId'),
  KEY 'IFK_InvoiceCustomerId' ('CustomerId'),
  CONSTRAINT 'FK_InvoiceCustomerId' FOREIGN KEY ('CustomerId') REFERENCES 'Customer' ('CustomerId') ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=413 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'Invoice'
--

LOCK TABLES 'Invoice' WRITE;
/*!40000 ALTER TABLE 'Invoice' DISABLE KEYS */;
INSERT INTO 'Invoice' VALUES (1,2,'2009-01-01 00:00:00','Theodor-Heuss-Straße 34','Stuttgart',NULL,'Germany','70174',1.98),(2,4,'2009-01-02 00:00:00','Ullevålsveien 14','Oslo',NULL,'Norway','0171',3.96),(3,8,'2009-01-03 00:00:00','Grétrystraat 63','Brussels',NULL,'Belgium','1000',5.94),(4,14,'2009-01-06 00:00:00','8210 111 ST NW','Edmonton','AB','Canada','T6G 2C7',8.91),(5,23,'2009-01-11 00:00:00','69 Salem Street','Boston','MA','USA','2113',13.86),(6,37,'2009-01-19 00:00:00','Berger Straße 10','Frankfurt',NULL,'Germany','60316',0.99),(7,38,'2009-02-01 00:00:00','Barbarossastraße 19','Berlin',NULL,'Germany','10779',1.98),(8,40,'2009-02-01 00:00:00','8, Rue Hanovre','Paris',NULL,'France','75002',1.98),(9,42,'2009-02-02 00:00:00','9, Place Louis Barthou','Bordeaux',NULL,'France','33000',3.96),(10,46,'2009-02-03 00:00:00','3 Chatham Street','Dublin','Dublin','Ireland',NULL,5.94),(11,52,'2009-02-06 00:00:00','202 Hoxton Street','London',NULL,'United Kingdom','N1 5LH',8.91),(12,2,'2009-02-11 00:00:00','Theodor-Heuss-Straße 34','Stuttgart',NULL,'Germany','70174',13.86),(13,16,'2009-02-19 00:00:00','1600 Amphitheatre Parkway','Mountain View','CA','USA','94043-1351',0.99),(14,17,'2009-03-04 00:00:00','1 Microsoft Way','Redmond','WA','USA','98052-8300',1.98),(15,19,'2009-03-04 00:00:00','1 Infinite Loop','Cupertino','CA','USA','95014',1.98),(16,21,'2009-03-05 00:00:00','801 W 4th Street','Reno','NV','USA','89503',3.96),(17,25,'2009-03-06 00:00:00','319 N. Frances Street','Madison','WI','USA','53703',5.94),(18,31,'2009-03-09 00:00:00','194A Chain Lake Drive','Halifax','NS','Canada','B3S 1C5',8.91),(19,40,'2009-03-14 00:00:00','8, Rue Hanovre','Paris',NULL,'France','75002',13.86);
/*!40000 ALTER TABLE 'Invoice' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'InvoiceLine'
--

DROP TABLE IF EXISTS 'InvoiceLine';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'InvoiceLine' (
  'InvoiceLineId' int(11) NOT NULL AUTO_INCREMENT,
  'InvoiceId' int(11) NOT NULL,
  'TrackId' int(11) NOT NULL,
  'UnitPrice' decimal(10,2) NOT NULL,
  'Quantity' int(11) NOT NULL,
  PRIMARY KEY ('InvoiceLineId'),
  KEY 'IFK_InvoiceLineInvoiceId' ('InvoiceId'),
  KEY 'IFK_InvoiceLineTrackId' ('TrackId'),
  CONSTRAINT 'FK_InvoiceLineTrackId' FOREIGN KEY ('TrackId') REFERENCES 'Track' ('TrackId') ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT 'FK_InvoiceLineInvoiceId' FOREIGN KEY ('InvoiceId') REFERENCES 'Invoice' ('InvoiceId') ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2241 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'InvoiceLine'
--

LOCK TABLES 'InvoiceLine' WRITE;
/*!40000 ALTER TABLE 'InvoiceLine' DISABLE KEYS */;
INSERT INTO 'InvoiceLine' VALUES (1,1,2,0.99,1),(2,1,4,0.99,1),(3,2,6,0.99,1),(4,2,8,0.99,1),(5,2,10,0.99,1),(6,2,12,0.99,1),(7,3,16,0.99,1),(8,3,20,0.99,1),(9,3,24,0.99,1),(10,3,28,0.99,1),(11,3,32,0.99,1),(12,3,36,0.99,1),(13,4,42,0.99,1),(14,4,48,0.99,1),(15,4,54,0.99,1),(16,4,60,0.99,1);
/*!40000 ALTER TABLE 'InvoiceLine' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'MediaType'
--

DROP TABLE IF EXISTS 'MediaType';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'MediaType' (
  'MediaTypeId' int(11) NOT NULL AUTO_INCREMENT,
  'Name' varchar(120) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY ('MediaTypeId')
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'MediaType'
--

LOCK TABLES 'MediaType' WRITE;
/*!40000 ALTER TABLE 'MediaType' DISABLE KEYS */;
INSERT INTO 'MediaType' VALUES (1,'MPEG audio file'),(2,'Protected AAC audio file'),(3,'Protected MPEG-4 video file'),(4,'Purchased AAC audio file'),(5,'AAC audio file');
/*!40000 ALTER TABLE 'MediaType' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'Playlist'
--

DROP TABLE IF EXISTS 'Playlist';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'Playlist' (
  'PlaylistId' int(11) NOT NULL AUTO_INCREMENT,
  'Name' varchar(120) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY ('PlaylistId')
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'Playlist'
--

LOCK TABLES 'Playlist' WRITE;
/*!40000 ALTER TABLE 'Playlist' DISABLE KEYS */;
INSERT INTO 'Playlist' VALUES (1,'Music'),(2,'Movies'),(3,'TV Shows'),(4,'Audiobooks'),(5,'90’s Music'),(6,'Audiobooks'),(7,'Movies'),(8,'Music'),(9,'Music Videos'),(10,'TV Shows'),(11,'Brazilian Music'),(12,'Classical'),(13,'Classical 101 - Deep Cuts'),(14,'Classical 101 - Next Steps'),(15,'Classical 101 - The Basics'),(16,'Grunge'),(17,'Heavy Metal Classic'),(18,'On-The-Go 1');
/*!40000 ALTER TABLE 'Playlist' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'PlaylistTrack'
--

DROP TABLE IF EXISTS 'PlaylistTrack';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'PlaylistTrack' (
  'PlaylistId' int(11) NOT NULL,
  'TrackId' int(11) NOT NULL,
  'PlaylistTrackId' int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY ('PlaylistTrackId'),
  KEY 'FK_PlaylistTrackId1' ('PlaylistTrackId'),
  KEY 'FK_PlaylistTrackPlaylistId1' ('PlaylistId'),
  KEY 'FK_PlaylistTrackTrackId1' ('TrackId'),
  CONSTRAINT 'FK_PlaylistTrackPlaylistId1' FOREIGN KEY ('PlaylistId') REFERENCES 'Playlist' ('PlaylistId') ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT 'FK_PlaylistTrackTrackId1' FOREIGN KEY ('TrackId') REFERENCES 'Track' ('TrackId') ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=128 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'PlaylistTrack'
--

LOCK TABLES 'PlaylistTrack' WRITE;
/*!40000 ALTER TABLE 'PlaylistTrack' DISABLE KEYS */;
INSERT INTO 'PlaylistTrack' VALUES (1,1,1),(1,2,2),(1,3,3),(5,3,4),(1,4,5),(5,4,6),(1,5,7),(5,5,8),(1,6,9),(1,7,10),(1,8,11),(1,9,12),(1,10,13),(1,11,14),(1,12,15),(1,13,16),(1,14,17),(1,15,18),(1,16,19),(1,17,20),(1,18,21),(1,19,22),(1,20,23),(1,21,24),(1,22,25),(1,23,26),(5,23,27),(1,24,28),(5,24,29),(1,25,30),(5,25,31),(1,26,32),(5,26,33),(1,27,34),(5,27,35),(1,28,36),(5,28,37),(1,29,38),(5,29,39),(1,30,40),(5,30,41),(1,31,42),(5,31,43),(1,32,44),(5,32,45),(1,33,46),(5,33,47),(1,34,48),(5,34,49),(1,35,50),(5,35,51),(1,36,52),(5,36,53),(1,37,54),(5,37,55),(1,38,56),(5,38,57),(1,39,58),(5,39,59),(1,40,60),(5,40,61),(1,41,62),(5,41,63),(1,42,64),(5,42,65),(1,43,66),(5,43,67),(1,44,68),(5,44,69),(1,45,70),(5,45,71),(1,46,72),(5,46,73),(1,47,74),(5,47,75),(1,48,76),(5,48,77),(1,49,78),(5,49,79),(1,50,80),(5,50,81),(1,51,82),(5,51,83),(1,52,84),(5,52,85),(1,53,86),(5,53,87),(1,54,88),(5,54,89),(1,55,90),(5,55,91),(1,56,92),(5,56,93),(1,57,94),(5,57,95),(1,58,96),(5,58,97),(1,59,98),(5,59,99),(1,60,100),(5,60,101),(1,61,102),(5,61,103),(1,62,104),(5,62,105),(1,63,106);
/*!40000 ALTER TABLE 'PlaylistTrack' ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table 'Track'
--

DROP TABLE IF EXISTS 'Track';
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE 'Track' (
  'TrackId' int(11) NOT NULL AUTO_INCREMENT,
  'Name' varchar(200) CHARACTER SET utf8 NOT NULL,
  'AlbumId' int(11) DEFAULT NULL,
  'MediaTypeId' int(11) NOT NULL,
  'GenreId' int(11) DEFAULT NULL,
  'Composer' varchar(220) CHARACTER SET utf8 DEFAULT NULL,
  'Milliseconds' int(11) NOT NULL,
  'Bytes' int(11) DEFAULT NULL,
  'UnitPrice' decimal(10,2) NOT NULL,
  PRIMARY KEY ('TrackId'),
  KEY 'IFK_TrackAlbumId' ('AlbumId'),
  KEY 'IFK_TrackGenreId' ('GenreId'),
  KEY 'IFK_TrackMediaTypeId' ('MediaTypeId'),
  CONSTRAINT 'FK_TrackMediaTypeId' FOREIGN KEY ('MediaTypeId') REFERENCES 'MediaType' ('MediaTypeId') ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT 'FK_TrackAlbumId' FOREIGN KEY ('AlbumId') REFERENCES 'Album' ('AlbumId') ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT 'FK_TrackGenreId' FOREIGN KEY ('GenreId') REFERENCES 'Genre' ('GenreId') ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=3504 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table 'Track'
--

LOCK TABLES 'Track' WRITE;
/*!40000 ALTER TABLE 'Track' DISABLE KEYS */;
INSERT INTO 'Track' VALUES (1,'For Those About To Rock (We Salute You)',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',343719,11170334,0.99),(2,'Balls to the Wall',2,2,1,NULL,342562,5510424,0.99),(3,'Fast As a Shark',3,2,1,'F. Baltes, S. Kaufman, U. Dirkscneider & W. Hoffman',230619,3990994,0.99),(4,'Restless and Wild',3,2,1,'F. Baltes, R.A. Smith-Diesel, S. Kaufman, U. Dirkscneider & W. Hoffman',252051,4331779,0.99),(5,'Princess of the Dawn',3,2,1,'Deaffy & R.A. Smith-Diesel',375418,6290521,0.99),(6,'Put The Finger On You',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',205662,6713451,0.99),(7,'Let\'s Get It Up',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',233926,7636561,0.99),(8,'Inject The Venom',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',210834,6852860,0.99),(9,'Snowballed',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',203102,6599424,0.99),(10,'Evil Walks',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',263497,8611245,0.99),(11,'C.O.D.',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',199836,6566314,0.99),(12,'Breaking The Rules',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',263288,8596840,0.99),(13,'Night Of The Long Knives',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',205688,6706347,0.99),(14,'Spellbound',1,1,1,'Angus Young, Malcolm Young, Brian Johnson',270863,8817038,0.99),(15,'Go Down',4,1,1,'AC/DC',331180,10847611,0.99),(16,'Dog Eat Dog',4,1,1,'AC/DC',215196,7032162,0.99),(17,'Let There Be Rock',4,1,1,'AC/DC',366654,12021261,0.99),(18,'Bad Boy Boogie',4,1,1,'AC/DC',267728,8776140,0.99),(19,'Problem Child',4,1,1,'AC/DC',325041,10617116,0.99),(20,'Overdose',4,1,1,'AC/DC',369319,12066294,0.99),(21,'Hell Ain\'t A Bad Place To Be',4,1,1,'AC/DC',254380,8331286,0.99),(22,'Whole Lotta Rosie',4,1,1,'AC/DC',323761,10547154,0.99),(23,'Walk On Water',5,1,1,'Steven Tyler, Joe Perry, Jack Blades, Tommy Shaw',295680,9719579,0.99),(24,'Love In An Elevator',5,1,1,'Steven Tyler, Joe Perry',321828,10552051,0.99),(25,'Rag Doll',5,1,1,'Steven Tyler, Joe Perry, Jim Vallance, Holly Knight',264698,8675345,0.99),(26,'What It Takes',5,1,1,'Steven Tyler, Joe Perry, Desmond Child',310622,10144730,0.99),(27,'Dude (Looks Like A Lady)',5,1,1,'Steven Tyler, Joe Perry, Desmond Child',264855,8679940,0.99),(28,'Janie\'s Got A Gun',5,1,1,'Steven Tyler, Tom Hamilton',330736,10869391,0.99),(29,'Cryin\'',5,1,1,'Steven Tyler, Joe Perry, Taylor Rhodes',309263,10056995,0.99),(30,'Amazing',5,1,1,'Steven Tyler, Richie Supa',356519,11616195,0.99),(31,'Blind Man',5,1,1,'Steven Tyler, Joe Perry, Taylor Rhodes',240718,7877453,0.99),(32,'Deuces Are Wild',5,1,1,'Steven Tyler, Jim Vallance',215875,7074167,0.99),(33,'The Other Side',5,1,1,'Steven Tyler, Jim Vallance',244375,7983270,0.99),(34,'Crazy',5,1,1,'Steven Tyler, Joe Perry, Desmond Child',316656,10402398,0.99),(35,'Eat The Rich',5,1,1,'Steven Tyler, Joe Perry, Jim Vallance',251036,8262039,0.99),(36,'Angel',5,1,1,'Steven Tyler, Desmond Child',307617,9989331,0.99),(37,'Livin\' On The Edge',5,1,1,'Steven Tyler, Joe Perry, Mark Hudson',381231,12374569,0.99),(38,'All I Really Want',6,1,1,'Alanis Morissette & Glenn Ballard',284891,9375567,0.99),(39,'You Oughta Know',6,1,1,'Alanis Morissette & Glenn Ballard',249234,8196916,0.99),(40,'Perfect',6,1,1,'Alanis Morissette & Glenn Ballard',188133,6145404,0.99),(41,'Hand In My Pocket',6,1,1,'Alanis Morissette & Glenn Ballard',221570,7224246,0.99),(42,'Right Through You',6,1,1,'Alanis Morissette & Glenn Ballard',176117,5793082,0.99),(43,'Forgiven',6,1,1,'Alanis Morissette & Glenn Ballard',300355,9753256,0.99),(44,'You Learn',6,1,1,'Alanis Morissette & Glenn Ballard',239699,7824837,0.99),(45,'Head Over Feet',6,1,1,'Alanis Morissette & Glenn Ballard',267493,8758008,0.99),(46,'Mary Jane',6,1,1,'Alanis Morissette & Glenn Ballard',280607,9163588,0.99),(47,'Ironic',6,1,1,'Alanis Morissette & Glenn Ballard',229825,7598866,0.99),(48,'Not The Doctor',6,1,1,'Alanis Morissette & Glenn Ballard',227631,7604601,0.99),(49,'Wake Up',6,1,1,'Alanis Morissette & Glenn Ballard',293485,9703359,0.99),(50,'You Oughta Know (Alternate)',6,1,1,'Alanis Morissette & Glenn Ballard',491885,16008629,0.99),(51,'We Die Young',7,1,1,'Jerry Cantrell',152084,4925362,0.99),(52,'Man In The Box',7,1,1,'Jerry Cantrell, Layne Staley',286641,9310272,0.99),(53,'Sea Of Sorrow',7,1,1,'Jerry Cantrell',349831,11316328,0.99),(54,'Bleed The Freak',7,1,1,'Jerry Cantrell',241946,7847716,0.99),(55,'I Can\'t Remember',7,1,1,'Jerry Cantrell, Layne Staley',222955,7302550,0.99),(56,'Love, Hate, Love',7,1,1,'Jerry Cantrell, Layne Staley',387134,12575396,0.99),(57,'It Ain\'t Like That',7,1,1,'Jerry Cantrell, Michael Starr, Sean Kinney',277577,8993793,0.99),(58,'Sunshine',7,1,1,'Jerry Cantrell',284969,9216057,0.99),(59,'Put You Down',7,1,1,'Jerry Cantrell',196231,6420530,0.99),(60,'Confusion',7,1,1,'Jerry Cantrell, Michael Starr, Layne Staley',344163,11183647,0.99),(61,'I Know Somethin (Bout You)',7,1,1,'Jerry Cantrell',261955,8497788,0.99),(62,'Real Thing',7,1,1,'Jerry Cantrell, Layne Staley',243879,7937731,0.99),(63,'Desafinado',8,1,2,NULL,185338,5990473,0.99);
/*!40000 ALTER TABLE 'Track' ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-02-17 15:49:52

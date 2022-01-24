-- MySQL dump 10.13  Distrib 8.0.23, for Win64 (x86_64)
--
-- Host: localhost    Database: bdsegundavidaroleplay
-- ------------------------------------------------------
-- Server version	8.0.23

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `apreensoes`
--

DROP TABLE IF EXISTS `apreensoes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `apreensoes` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Data` datetime DEFAULT NULL,
  `Veiculo` int DEFAULT NULL,
  `PersonagemPolicial` int DEFAULT NULL,
  `Valor` int DEFAULT NULL,
  `Motivo` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `DataPagamento` datetime DEFAULT NULL,
  `Faccao` int NOT NULL DEFAULT '0',
  `Descricao` text COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `apreensoes`
--

LOCK TABLES `apreensoes` WRITE;
/*!40000 ALTER TABLE `apreensoes` DISABLE KEYS */;
/*!40000 ALTER TABLE `apreensoes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `armarios`
--

DROP TABLE IF EXISTS `armarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `armarios` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `PosX` float NOT NULL DEFAULT '0',
  `PosY` float NOT NULL DEFAULT '0',
  `PosZ` float NOT NULL DEFAULT '0',
  `Dimensao` bigint NOT NULL DEFAULT '0',
  `Faccao` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `armarios`
--

LOCK TABLES `armarios` WRITE;
/*!40000 ALTER TABLE `armarios` DISABLE KEYS */;
/*!40000 ALTER TABLE `armarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `armarioscomponentes`
--

DROP TABLE IF EXISTS `armarioscomponentes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `armarioscomponentes` (
  `Codigo` int NOT NULL,
  `Arma` bigint NOT NULL,
  `Componente` bigint NOT NULL,
  PRIMARY KEY (`Codigo`,`Arma`,`Componente`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `armarioscomponentes`
--

LOCK TABLES `armarioscomponentes` WRITE;
/*!40000 ALTER TABLE `armarioscomponentes` DISABLE KEYS */;
/*!40000 ALTER TABLE `armarioscomponentes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `armariositens`
--

DROP TABLE IF EXISTS `armariositens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `armariositens` (
  `Codigo` int NOT NULL,
  `Arma` bigint NOT NULL DEFAULT '0',
  `Municao` int NOT NULL DEFAULT '0',
  `Estoque` int NOT NULL DEFAULT '0',
  `Pintura` tinyint NOT NULL DEFAULT '0',
  `Rank` int NOT NULL DEFAULT '0',
  `Componentes` text COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Codigo`,`Arma`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `armariositens`
--

LOCK TABLES `armariositens` WRITE;
/*!40000 ALTER TABLE `armariositens` DISABLE KEYS */;
/*!40000 ALTER TABLE `armariositens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `banimentos`
--

DROP TABLE IF EXISTS `banimentos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `banimentos` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Data` datetime DEFAULT NULL,
  `Expiracao` datetime DEFAULT NULL,
  `Usuario` int DEFAULT '0',
  `SocialClub` bigint DEFAULT '0',
  `Motivo` varchar(255) COLLATE utf8mb4_general_ci DEFAULT '',
  `Usuariostaff` int DEFAULT '0',
  `HardwareIdHash` bigint NOT NULL DEFAULT '0',
  `HardwareIdExHash` bigint NOT NULL DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `banimentos`
--

LOCK TABLES `banimentos` WRITE;
/*!40000 ALTER TABLE `banimentos` DISABLE KEYS */;
/*!40000 ALTER TABLE `banimentos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `blips`
--

DROP TABLE IF EXISTS `blips`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `blips` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(50) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `PosX` float NOT NULL,
  `PosY` float NOT NULL,
  `PosZ` float NOT NULL,
  `Tipo` int NOT NULL DEFAULT '0',
  `Cor` int NOT NULL DEFAULT '0',
  `Inativo` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=132 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `blips`
--

LOCK TABLES `blips` WRITE;
/*!40000 ALTER TABLE `blips` DISABLE KEYS */;
INSERT INTO `blips` VALUES (2,'Posto de Gasolina',1207.99,-1402.9,34.5078,361,73,_binary '\0'),(3,'Posto de Gasolina',1181.75,-329.442,68.5275,361,73,_binary '\0'),(4,'Posto de Gasolina',-526.155,-1210.08,17.7619,361,73,_binary '\0'),(5,'Posto de Gasolina',-1437.36,-276.696,45.7855,361,73,_binary '\0'),(6,'Posto de Gasolina',-2099.07,-319.457,12.601,361,73,_binary '\0'),(7,'Posto de Gasolina',264.919,-1262.49,29.2929,361,73,_binary '\0'),(8,'Posto de Gasolina',-319.468,-1471.06,30.5484,361,73,_binary '\0'),(9,'Posto de Gasolina',2578.9,361.869,108.04,361,73,_binary '\0'),(10,'Posto de Gasolina',1783.23,3330.3,40.8309,361,73,_binary '\0'),(11,'Posto de Gasolina',2003.29,3776.27,31.7577,361,73,_binary '\0'),(12,'Posto de Gasolina',1702.66,6418.4,32.217,361,73,_binary '\0'),(13,'Posto de Gasolina',183.013,6600.8,31.4279,361,73,_binary '\0'),(14,'Posto de Gasolina',-2554.8,2331.92,32.6327,361,73,_binary '\0'),(15,'Posto de Gasolina',50.2413,2781.29,57.4591,361,73,_binary '\0'),(16,'Posto de Gasolina',264.514,2609.6,44.414,361,73,_binary '\0'),(17,'Posto de Gasolina',1205.58,2659.16,37.394,361,73,_binary '\0'),(18,'Posto de Gasolina',817.679,-1029.47,25.6174,361,73,_binary '\0'),(19,'LSPD',827.447,-1290.13,28.2407,60,38,_binary ''),(20,'LSPD',435.499,-981.99,29.9843,60,38,_binary '\0'),(21,'LSPD',360.768,-1584.57,28.577,60,38,_binary ''),(22,'LSPD',638.959,1.21253,82.0714,60,38,_binary ''),(23,'LSPD',-1108.49,-845.143,18.6009,60,38,_binary ''),(25,'Hospital',342.589,-1398.51,31.7925,61,75,_binary ''),(26,'Posto de Gasolina',-1799.09,802.799,138.651,361,73,_binary '\0'),(27,'Hospital',1150.8,-1530.08,35.3771,61,75,_binary ''),(28,'Posto de Gasolina',-70.4875,-1760.45,29.534,361,73,_binary '\0'),(29,'Posto de Gasolina',620.829,268.963,103.089,361,73,_binary '\0'),(30,'Posto de Gasolina',-94.1692,6419.44,31.4895,361,73,_binary '\0'),(31,'LSFD',209.082,-1648.46,29.0865,60,75,_binary '\0'),(32,'LSFD',1201.51,-1474.28,34.1436,60,75,_binary ''),(33,'Posto de Gasolina',-724.197,-933.784,19.2139,361,73,_binary '\0'),(35,'Banco',149.435,-1040.36,29.3721,500,69,_binary '\0'),(36,'Loja de Roupas',425.516,-806.092,29.4912,73,4,_binary '\0'),(37,'Loja de Conveniência',-707.416,-913.433,19.2156,52,4,_binary '\0'),(38,'Loja de Conveniência',-1222.93,-907.209,12.3264,52,4,_binary '\0'),(39,'Banco',313.961,-278.997,54.1708,500,69,_binary '\0'),(40,'Loja de Roupas',-709.636,-153.466,37.4151,73,4,_binary '\0'),(41,'Banco',-351.212,-49.8107,49.0426,500,69,_binary '\0'),(42,'Loja de Roupas',125.663,-223.969,54.5578,73,4,_binary '\0'),(43,'Loja de Roupas',-163.41,-302.953,39.7333,73,4,_binary '\0'),(44,'Barbearia',-33.8572,-151.929,57.0834,71,4,_binary '\0'),(45,'Loja de Conveniência',1163.46,-323.099,69.2051,52,4,_binary '\0'),(46,'Tequi-La-La',-564.961,275.005,83.0196,93,4,_binary '\0'),(47,'Loja de Conveniência',-47.9345,-1757.35,29.421,52,4,_binary '\0'),(49,'Loja de Roupas',-818.356,-1073.48,11.3281,73,4,_binary '\0'),(50,'Loja de Conveniência',-2967.88,390.266,15.0433,52,4,_binary '\0'),(51,'Banco',-2962.61,482.281,15.701,500,69,_binary '\0'),(52,'Loja de Roupas',-3170.57,1043.85,20.8632,73,4,_binary '\0'),(53,'Estúdio de Tatuagens',-3170.99,1076.54,20.8292,75,4,_binary ''),(54,'Loja de Conveniência',-3243.03,1001.34,12.8307,52,4,_binary '\0'),(55,'Loja de Conveniência',-3040.32,585.45,7.90893,52,4,_binary '\0'),(56,'Loja de Conveniência',2556.2,382.135,108.623,52,4,_binary '\0'),(57,'Barbearia',-279.417,6227.72,31.7037,71,4,_binary '\0'),(58,'Estúdio de Tatuagens',-294.462,6201.05,31.4872,75,4,_binary ''),(59,'Banco',-112.531,6469.64,31.6267,500,69,_binary '\0'),(60,'Loja de Roupas',1.51764,6510.79,31.878,73,4,_binary '\0'),(61,'LSPD',-447.525,6013.69,31.7164,60,38,_binary ''),(62,'Yellow Jack',1990.57,3053.83,47.215,93,4,_binary '\0'),(63,'LSFD',1698.11,3585.76,35.2481,60,75,_binary ''),(64,'Loja de Conveniência',1729.4,6415.46,35.0372,52,4,_binary '\0'),(65,'Hospital',1838.25,3673.26,34.2766,61,75,_binary ''),(66,'LSPD',1855.33,3683.38,34.2675,60,38,_binary ''),(67,'Barbearia',1933.17,3730.41,32.8544,71,4,_binary '\0'),(68,'Loja de Conveniência',1961.3,3740.47,32.3437,52,4,_binary '\0'),(69,'Loja de Conveniência',1392.85,3605.03,34.9809,52,4,_binary '\0'),(70,'Loja de Roupas',1693.1,4819.15,42.0631,73,4,_binary '\0'),(71,'Loja de Conveniência',1698.29,4924.35,42.0637,52,4,_binary '\0'),(72,'Loja de Roupas',-1098.32,2712.37,19.1083,73,4,_binary '\0'),(73,'Loja de Conveniência',547.874,2670.56,42.1565,52,4,_binary '\0'),(74,'Loja de Roupas',614.276,2762.92,42.0881,73,4,_binary '\0'),(75,'Loja de Conveniência',1165,2709.36,38.1577,52,4,_binary '\0'),(76,'Banco',1175.85,2706.8,38.0941,500,69,_binary '\0'),(77,'Loja de Roupas',1200.21,2708.74,38.2226,73,4,_binary '\0'),(78,'LSFD',-380.96,6119.4,31.3409,60,75,_binary ''),(79,'Hospital',-248.365,6332.24,32.4262,61,75,_binary ''),(80,'Loja de Conveniência',-1486.8,-379.526,40.1634,52,4,_binary '\0'),(81,'Barbearia',-1282.9,-1118.2,6.99993,71,4,_binary '\0'),(82,'Estúdio de Tatuagens',-1153.53,-1424.37,4.95446,75,4,_binary ''),(83,'Barbearia',137.66,-1708.73,29.3003,71,4,_binary '\0'),(84,'Loja de Conveniência',25.747,-1346.91,29.497,52,4,_binary '\0'),(85,'Loja de Conveniência',1135.57,-981.754,46.4158,52,4,_binary '\0'),(86,'Loja de Conveniência',374.074,327.205,103.566,52,4,_binary '\0'),(87,'Loja de Roupas',78.1354,-1389.36,29.3761,73,4,_binary '\0'),(88,'Loja de Roupas',-1192.74,-767.751,17.3187,73,4,_binary '\0'),(89,'Concessionária de Helicópteros',-753.434,-1512.39,5.02002,43,51,_binary '\0'),(90,'Concessionária de Barcos',-787.117,-1354.87,5.15027,427,51,_binary '\0'),(91,'Concessionária de Aviões',1735.17,3294.88,41.188,251,51,_binary ''),(92,'Concessionária Industrial',474.086,-1951.73,24.6233,67,51,_binary ''),(93,'Downtown Cab Co.',895.125,-179.27,74.7004,56,73,_binary '\0'),(94,'Loja de Roupas',-1450.58,-237.695,49.8106,73,4,_binary '\0'),(95,'Barbearia',-820.254,-186.331,37.5689,71,4,_binary '\0'),(96,'Banco',259.059,202.67,106.212,500,69,_binary '\0'),(97,'Barbearia',137.512,-1709.3,29.2996,71,4,_binary '\0'),(102,'Posto de Gasolina',174.91,-1561.65,29.2461,361,73,_binary '\0'),(103,'Hospital',297.27,-584.057,43.1304,61,75,_binary '\0'),(104,'Loja de Conveniência',-1820.89,793.266,138.096,52,4,_binary '\0'),(105,'Pátio de Apreensão',409.055,-1622.77,29.2799,524,4,_binary '\0'),(106,'Hospital',-447.666,-341.275,34.4865,61,75,_binary ''),(107,'LSPD',-561.956,-131.09,38.4293,60,38,_binary ''),(109,'DMV',-286.286,281.631,89.8718,498,4,_binary '\0'),(110,'Concessionária de Aviões',-941.09,-2954.19,13.9297,251,51,_binary '\0'),(111,'Prefeitura',-545.064,-203.631,38.2102,419,4,_binary '\0'),(112,'Central de Mecânicos',-1148.23,-1999.57,13.1714,633,3,_binary '\0'),(113,'Estúdio de Tatuagens',321.191,180.356,103.571,75,4,_binary ''),(114,'Concessionária Industrial',967.464,-1829.33,31.2344,67,51,_binary '\0'),(115,'Luxury Autos',-802.892,-223.82,37.2161,225,51,_binary '\0'),(118,'Sanders Motorcycles Dealership',268.734,-1155.38,29.2799,226,51,_binary '\0'),(119,'Premium Deluxe Motorsport',-56.0176,-1099.28,26.4154,225,51,_binary '\0'),(120,'Dinka Oriental Autos',-903.798,-225.086,40.0132,225,51,_binary '\0'),(122,'Benefactor Euro Cars',-68.8615,63.5736,71.8931,225,51,_binary '\0'),(123,'Albany & Declasse Autos',-40.7736,-1674.79,29.4652,225,51,_binary '\0'),(124,'VAPID',-202.18,-1158.61,23.8037,225,51,_binary '\0'),(125,'Centro de Reciclagem',-355.121,-1513.41,27.7128,318,25,_binary '\0'),(126,'LSFD',-660.29,-77.0901,38.7832,60,75,_binary ''),(127,'Concessionária de Bicicletas',-1108.51,-1693.7,4.35901,226,51,_binary '\0'),(128,'Bahama Mamas',-1388.35,-587.156,30.2065,93,4,_binary '\0'),(129,'Ferro Velho',-429.666,-1719.52,19.0688,527,10,_binary '\0'),(130,'Weazel News',-598.418,-929.723,23.8542,564,4,_binary '\0'),(131,'Lifeinvader',-1081.67,-260.967,37.7891,77,1,_binary '\0');
/*!40000 ALTER TABLE `blips` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `confiscos`
--

DROP TABLE IF EXISTS `confiscos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `confiscos` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Data` datetime NOT NULL,
  `Personagem` int NOT NULL,
  `PersonagemPolicial` int NOT NULL,
  `Armas` text COLLATE utf8mb4_general_ci NOT NULL,
  `Descricao` text COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `confiscos`
--

LOCK TABLES `confiscos` WRITE;
/*!40000 ALTER TABLE `confiscos` DISABLE KEYS */;
/*!40000 ALTER TABLE `confiscos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `faccoes`
--

DROP TABLE IF EXISTS `faccoes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `faccoes` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(50) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Tipo` int NOT NULL DEFAULT '0',
  `Cor` varchar(6) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `RankGestor` int NOT NULL DEFAULT '0',
  `RankLider` int NOT NULL DEFAULT '0',
  `Slots` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `faccoes`
--

LOCK TABLES `faccoes` WRITE;
/*!40000 ALTER TABLE `faccoes` DISABLE KEYS */;
INSERT INTO `faccoes` VALUES (1,'Los Santos Police Department',1,'8c8cff',10,18,30),(2,'Los Santos Fire Department',2,'e0573f',7,13,20),(3,'Los Santos Government',4,'F1F50E',24,31,0),(4,'Weazel News',5,'D96D00',15,16,10),(5,'Faction Team',4,'FFFFFF',0,1,0);
/*!40000 ALTER TABLE `faccoes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ligacoes911`
--

DROP TABLE IF EXISTS `ligacoes911`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ligacoes911` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Tipo` int NOT NULL DEFAULT '0',
  `Data` datetime DEFAULT NULL,
  `Celular` int NOT NULL DEFAULT '0',
  `PosX` float NOT NULL DEFAULT '0',
  `PosY` float NOT NULL DEFAULT '0',
  `PosZ` float NOT NULL DEFAULT '0',
  `Mensagem` varchar(500) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Localizacao` varchar(255) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ligacoes911`
--

LOCK TABLES `ligacoes911` WRITE;
/*!40000 ALTER TABLE `ligacoes911` DISABLE KEYS */;
/*!40000 ALTER TABLE `ligacoes911` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `logs`
--

DROP TABLE IF EXISTS `logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `logs` (
  `Codigo` bigint NOT NULL AUTO_INCREMENT,
  `Data` datetime DEFAULT NULL,
  `Tipo` int DEFAULT '0',
  `PersonagemOrigem` int DEFAULT '0',
  `PersonagemDestino` int DEFAULT '0',
  `Descricao` text COLLATE utf8mb4_general_ci,
  `IPOrigem` varchar(25) COLLATE utf8mb4_general_ci DEFAULT '',
  `IPDestino` varchar(25) COLLATE utf8mb4_general_ci DEFAULT '',
  `SocialClubOrigem` bigint DEFAULT '0',
  `SocialClubDestino` bigint DEFAULT '0',
  `HardwareIdHashOrigem` bigint NOT NULL DEFAULT '0',
  `HardwareIdHashDestino` bigint NOT NULL DEFAULT '0',
  `HardwareIdExHashOrigem` bigint NOT NULL DEFAULT '0',
  `HardwareIdExHashDestino` bigint NOT NULL DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `logs`
--

LOCK TABLES `logs` WRITE;
/*!40000 ALTER TABLE `logs` DISABLE KEYS */;
/*!40000 ALTER TABLE `logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `multas`
--

DROP TABLE IF EXISTS `multas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `multas` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Data` datetime NOT NULL,
  `PersonagemMultado` int NOT NULL DEFAULT '0',
  `PersonagemPolicial` int NOT NULL DEFAULT '0',
  `Valor` int NOT NULL DEFAULT '0',
  `Motivo` varchar(255) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `DataPagamento` datetime DEFAULT NULL,
  `Descricao` text COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `multas`
--

LOCK TABLES `multas` WRITE;
/*!40000 ALTER TABLE `multas` DISABLE KEYS */;
/*!40000 ALTER TABLE `multas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `parametros`
--

DROP TABLE IF EXISTS `parametros`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `parametros` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `RecordeOnline` int DEFAULT '0',
  `ValorVagaVeiculo` int NOT NULL DEFAULT '0',
  `ValorCustosHospitalares` int NOT NULL DEFAULT '0',
  `ValorBarbearia` int DEFAULT '0',
  `ValorRoupas` int NOT NULL DEFAULT '0',
  `ValorLicencaMotorista` int NOT NULL DEFAULT '0',
  `ValorComponentes` int NOT NULL DEFAULT '0',
  `ValorCombustivel` int NOT NULL DEFAULT '0',
  `Paycheck` int NOT NULL DEFAULT '1',
  `ValorLicencaMotoristaRenovacao` int NOT NULL DEFAULT '0',
  `ValorAnuncio` int NOT NULL DEFAULT '0',
  `ValorExtraLixeiro` int NOT NULL DEFAULT '0',
  `Weather` int NOT NULL DEFAULT '0',
  `Blackout` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `parametros`
--

LOCK TABLES `parametros` WRITE;
/*!40000 ALTER TABLE `parametros` DISABLE KEYS */;
INSERT INTO `parametros` VALUES (1,1,2000,500,50,50,700,350,2,1,300,150,20,1,_binary '\0');
/*!40000 ALTER TABLE `parametros` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `perguntas`
--

DROP TABLE IF EXISTS `perguntas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `perguntas` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Nome` text COLLATE utf8mb4_general_ci NOT NULL,
  `RespostaCorreta` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `perguntas`
--

LOCK TABLES `perguntas` WRITE;
/*!40000 ALTER TABLE `perguntas` DISABLE KEYS */;
INSERT INTO `perguntas` VALUES (1,'Você está dirigindo seu carro tranquilamente e percebe que atrás de você está acontecendo uma perseguição policial indo na sua direção, o que você faz?\r\n',2),(2,'De acordo com as regras do servidor, a regra de número 11 se refere à:',9),(3,'No que se refere às regras do servidor, qual delas não permite  que você capote um veículo e continue fugindo?',13),(4,'Durante um roleplay um jogador está visivelmente quebrando uma regra do servidor e impactando negativamente o seu jogo. Qual é a melhor maneira para prosseguir diante dessa situação?',15),(5,'O seu personagem está discutindo com um outro por causa de um acidente de trânsito que acabou de acontecer, entretanto você resolve chamar um amigo via Discord para que ele te auxilie na discussão. Qual regra do servidor você quebrou?',4),(6,'O seu personagem é atualmente um membro ligado à uma organização criminosa de nível hierárquico alto dentro do cenário de interpretação. Você descobre que a polícia fará uma batida em um dos locais que seus capangas estão produzindo algo acerca do trabalho ilegal, que pode fazer com que a facção seja fechada. Entretanto, você recebeu essa informação via meios externos, por um amigo seu, sem que ambos tenham qualquer ligação em cunho interpretativo, in-character. Você retira seus capangas da operação, sem descobrir nada de forma IC. Você está quebrando qual dessas regras?',18),(7,'O que significam os parênteses ((olá)) ao redor de um texto?',21),(8,'Você está jogando normalmente quando entra em um conflito e acaba sendo xingado por outro jogador em um canal In Character, e no Out of Character você se sente ofendido, qual é sua reação?',23),(9,'No seu conhecimento sobre as regras de roleplay, é correto afirmar que:',28),(10,'Realizar uma ação forçada sobre determinado jogador e faltar com respeito com ele através do canal /b são duas transgressões, respectivamente, de:',31),(12,'Você está próximo de um grupo de pessoas que não conhece, então começa a disparar com uma arma de fogo sem motivo em direção aos mesmos. Neste cenário, você está cometendo qual tipo de infração conforme as regras do servidor?',33),(13,'Você é novo no servidor e não tem intenção de manter seu personagem no roleplay civil, ou seja, levar uma vida de bom cidadão. Você quer ir além e se juntar a alguma facção, então você vai:',36),(14,'Ter uma arma é uma das coisas mais legais no universo do GTA, tanto offline quanto online, isso se não for a parte mais legal. Indique qual opção abaixo representa uma forma INCORRETA de como conseguir uma arma e pessoas que deveriam ter uma:',38);
/*!40000 ALTER TABLE `perguntas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `personagens`
--

DROP TABLE IF EXISTS `personagens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `personagens` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(50) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Usuario` int NOT NULL DEFAULT '0',
  `DataRegistro` datetime NOT NULL,
  `IPRegistro` varchar(25) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `DataUltimoAcesso` datetime NOT NULL,
  `IPUltimoAcesso` varchar(25) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Skin` bigint NOT NULL DEFAULT '0',
  `PosX` float NOT NULL DEFAULT '0',
  `PosY` float NOT NULL DEFAULT '0',
  `PosZ` float NOT NULL DEFAULT '0',
  `Vida` int NOT NULL DEFAULT '0',
  `Colete` int NOT NULL DEFAULT '0',
  `Dimensao` bigint NOT NULL DEFAULT '0',
  `DataNascimento` date NOT NULL,
  `Online` bit(1) NOT NULL DEFAULT b'0',
  `TempoConectado` int NOT NULL DEFAULT '0',
  `Faccao` int NOT NULL DEFAULT '0',
  `Rank` int NOT NULL DEFAULT '0',
  `SocialClubRegistro` bigint DEFAULT '0',
  `SocialClubUltimoAcesso` bigint DEFAULT '0',
  `Dinheiro` int DEFAULT '0',
  `Celular` int DEFAULT '0',
  `Banco` int NOT NULL DEFAULT '0',
  `IPL` text COLLATE utf8mb4_general_ci NOT NULL,
  `CanalRadio` int NOT NULL DEFAULT '-1',
  `CanalRadio2` int NOT NULL DEFAULT '-1',
  `CanalRadio3` int NOT NULL DEFAULT '-1',
  `RotX` float NOT NULL DEFAULT '0',
  `RotY` float NOT NULL DEFAULT '0',
  `RotZ` float NOT NULL DEFAULT '0',
  `DataMorte` datetime DEFAULT NULL,
  `MotivoMorte` varchar(255) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Emprego` int NOT NULL DEFAULT '0',
  `InformacoesPersonalizacao` text COLLATE utf8mb4_general_ci NOT NULL,
  `HardwareIdHashRegistro` bigint NOT NULL DEFAULT '0',
  `HardwareIdExHashRegistro` bigint NOT NULL DEFAULT '0',
  `HardwareIdHashUltimoAcesso` bigint NOT NULL DEFAULT '0',
  `HardwareIdExHashUltimoAcesso` bigint NOT NULL DEFAULT '0',
  `Historia` text COLLATE utf8mb4_general_ci,
  `UsuariostaffAvaliador` int NOT NULL DEFAULT '0',
  `MotivoRejeicao` varchar(1000) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `StatusNamechange` int NOT NULL DEFAULT '0',
  `EtapaPersonalizacao` int NOT NULL DEFAULT '0',
  `DataExclusao` datetime DEFAULT NULL,
  `DataTerminoPrisao` datetime DEFAULT NULL,
  `DataValidadeLicencaMotorista` date DEFAULT NULL,
  `PolicialRevogacaoLicencaMotorista` mediumint NOT NULL DEFAULT '0',
  `Distintivo` int NOT NULL DEFAULT '0',
  `InformacoesRoupas` text COLLATE utf8mb4_general_ci NOT NULL,
  `InformacoesAcessorios` text COLLATE utf8mb4_general_ci NOT NULL,
  `InformacoesArmas` text COLLATE utf8mb4_general_ci NOT NULL,
  `InformacoesContatos` text COLLATE utf8mb4_general_ci NOT NULL,
  `PecasVeiculares` int NOT NULL DEFAULT '0',
  `DataUltimoUsoAnuncio` datetime DEFAULT NULL,
  `Mascara` int NOT NULL DEFAULT '0',
  `Roupa` int NOT NULL DEFAULT '0',
  `Poupanca` int NOT NULL DEFAULT '0',
  `PagamentoExtra` int NOT NULL DEFAULT '0',
  `InformacoesFerimentos` text COLLATE utf8mb4_general_ci NOT NULL,
  `TipoFerido` int NOT NULL DEFAULT '0',
  `UsuarioStaffAvaliando` int DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `personagens`
--

LOCK TABLES `personagens` WRITE;
/*!40000 ALTER TABLE `personagens` DISABLE KEYS */;
/*!40000 ALTER TABLE `personagens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pontos`
--

DROP TABLE IF EXISTS `pontos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pontos` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Tipo` int DEFAULT '0',
  `PosX` float DEFAULT '0',
  `PosY` float DEFAULT '0',
  `PosZ` float DEFAULT '0',
  `Configuracoes` varchar(500) COLLATE utf8mb4_general_ci DEFAULT '',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=172 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pontos`
--

LOCK TABLES `pontos` WRITE;
/*!40000 ALTER TABLE `pontos` DISABLE KEYS */;
INSERT INTO `pontos` VALUES (2,1,149.287,-1040.51,29.3741,''),(3,2,1135.63,-982.573,46.4158,''),(4,3,76.7009,-1389.38,29.3762,''),(5,3,-1450.58,-237.695,49.8106,''),(8,2,1163.21,-323.591,69.2051,''),(9,2,2557.11,382.099,108.623,''),(10,3,425.516,-806.093,29.4912,''),(11,2,-707.416,-913.433,19.2156,''),(12,1,313.962,-278.998,54.1708,''),(13,3,-709.635,-153.468,37.4151,''),(14,1,-351.212,-49.8118,49.0426,''),(15,3,125.663,-223.969,54.5578,''),(16,3,-163.41,-302.953,39.7333,''),(17,2,-47.9341,-1757.35,29.421,''),(18,3,-818.356,-1073.48,11.3281,''),(19,1,-2962.61,482.281,15.7019,''),(20,3,-3170.57,1043.85,20.8632,''),(21,2,-3243.03,1001.34,12.8307,''),(22,2,-3040.32,585.449,7.90893,''),(23,1,-113.149,6470.04,31.6267,''),(24,3,1.51784,6510.79,31.8783,''),(26,2,1729.4,6415.46,35.0372,''),(28,2,1961.3,3740.47,32.3437,''),(29,3,1693.1,4819.15,42.0631,''),(30,2,1698.29,4924.35,42.0637,''),(31,3,-1100.89,2710.98,19.1079,''),(32,2,547.873,2670.56,42.1565,''),(33,3,614.276,2762.92,42.0881,''),(34,2,1165,2709.36,38.1577,''),(35,1,1175.85,2706.8,38.0941,''),(36,3,1200.21,2708.74,38.2226,''),(37,2,25.7454,-1346.91,29.497,''),(38,2,374.073,327.205,103.566,''),(39,3,-1192.74,-767.751,17.3187,''),(49,2,-1820.89,793.266,138.096,''),(54,4,452.624,-1020.73,28.3363,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":1.5831648}'),(55,4,449.367,-981.138,43.6864,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":1.5336909}'),(56,5,402.026,-1632.32,29.2799,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":2.3252733}'),(57,6,408.963,-1622.8,29.2799,NULL),(58,7,136.985,-1707.98,29.2799,NULL),(59,8,459.811,-990.857,30.6783,NULL),(60,8,197.684,-1646.12,29.8022,NULL),(63,2,-2967.97,390.448,15.0417,NULL),(64,7,-33.3231,-152.176,57.0652,NULL),(65,7,-1282.79,-1118.2,6.98755,NULL),(66,10,-286.207,281.604,89.855,NULL),(68,9,441.073,-978.831,30.6783,NULL),(70,11,338.532,-583.859,74.1509,'{\"X\":309.87692,\"Y\":-603.03296,\"Z\":43.282104}'),(71,4,351.6,-587.789,74.1509,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":-1.8800083}'),(73,11,-69.3363,-802.998,44.2256,'{\"X\":-75.00659,\"Y\":-824.189,\"Z\":321.28723}'),(74,11,-75.0066,-824.189,321.287,'{\"X\":-69.33626,\"Y\":-802.9978,\"Z\":44.225586}'),(75,4,-577.688,-250.457,35.7333,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":0.49473903}'),(76,4,-725.314,-1443.89,4.29163,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":-0.69263464}'),(77,11,309.916,-602.941,43.2821,'{\"X\":338.53186,\"Y\":-583.8593,\"Z\":74.15088}'),(78,12,308.255,-595.253,43.2821,NULL),(80,2,-1487.17,-379.042,40.1479,NULL),(81,13,-429.666,-1719.52,19.0688,NULL),(83,4,216.791,-1633.16,29.2461,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":-0.7421085}'),(85,14,-168.211,-1412.14,30.9817,NULL),(86,14,-77.0901,-1383.72,29.3136,NULL),(87,14,1.5956,-1386.7,29.2799,NULL),(88,14,-17.2879,-1389.43,29.3978,NULL),(89,14,48.8835,-1386.44,29.3136,NULL),(90,14,96.567,-1439.67,29.2799,NULL),(91,14,75.5209,-1463.84,29.2461,NULL),(92,14,65.644,-1470.62,29.2799,NULL),(93,14,46.8791,-1478.8,29.2799,NULL),(94,14,-7.27912,-1477.05,30.5267,NULL),(95,14,-94.378,-1469.76,33.2902,NULL),(96,14,-114.409,-1452.96,33.4418,NULL),(97,14,-145.174,-1464.78,33.4755,NULL),(98,14,-171.705,-1459.83,31.6893,NULL),(99,14,-133.596,-1782.04,29.8191,NULL),(100,14,118.246,-1943.8,20.6527,NULL),(101,14,76.5363,-1929.92,20.8381,NULL),(102,14,130.668,-1886.41,23.5004,NULL),(103,14,190.246,-1915.7,22.6241,NULL),(104,14,153.244,-1974.75,18.4454,NULL),(105,14,286.378,-1811.8,27.1399,NULL),(106,14,432.158,-1551.24,29.7347,NULL),(107,14,468.765,-1585.09,29.2631,NULL),(108,14,464.123,-1540.92,29.7179,NULL),(109,14,488.954,-1511.17,29.5157,NULL),(110,14,301.213,-1285.93,30.392,NULL),(111,14,202.193,-1265.97,29.2124,NULL),(112,14,2.38681,-1351.44,29.3473,NULL),(113,14,-39.1121,-1352.07,29.3304,NULL),(114,14,-87.244,-1330.34,29.2799,NULL),(115,14,-87.3758,-1287.6,29.2968,NULL),(116,14,-168.976,-1294.81,31.2007,NULL),(117,14,174.87,-304.536,46.1465,NULL),(118,14,126.488,-288.949,46.2982,NULL),(119,14,-18.4483,-218.123,46.1633,NULL),(120,14,-27.2571,-78.0264,57.2505,NULL),(121,14,208.22,-166.642,56.3239,NULL),(122,14,1.79341,-54.7253,63.2828,NULL),(123,14,-101.921,45.5077,71.6066,NULL),(125,14,-623.051,293.512,82.0535,NULL),(126,14,-518.479,270.554,83.0645,NULL),(127,14,-259.464,294,91.5905,NULL),(128,14,143.538,195.244,106.57,NULL),(129,14,394.378,101.657,101.852,NULL),(130,14,645.007,138.119,91.3883,NULL),(131,14,394.734,269.314,103.015,NULL),(132,14,-1066.65,376.629,68.9443,NULL),(133,14,-1233.38,386.163,75.3641,NULL),(134,14,-1017.11,506.268,79.4586,NULL),(135,14,-1309.15,-280.404,39.6256,NULL),(136,14,-1384.64,-334.418,39.5751,NULL),(137,14,-1540.91,-394.378,41.9846,NULL),(138,14,-1558.25,-478.101,35.4301,NULL),(139,14,-1636.69,-1056.51,13.1377,NULL),(140,14,-1603.82,-814.167,9.98682,NULL),(141,14,-1989.56,-488.571,11.6044,NULL),(142,14,-1752.16,-377.947,45.759,NULL),(143,14,-1630.48,-358.774,48.3033,NULL),(144,14,21.7846,374.862,112.568,NULL),(145,14,195.27,341.275,106.048,NULL),(146,14,818.69,-99.2176,80.5875,NULL),(147,14,1180.23,-304.352,69.0791,NULL),(148,14,1099.56,-775.477,58.3458,NULL),(149,11,-1075.36,-253.49,44.0066,'{\"X\":-1075.3715,\"Y\":-253.42418,\"Z\":37.75537}'),(150,11,-1077.93,-254.796,44.0066,'{\"X\":-1078.1011,\"Y\":-254.87473,\"Z\":37.75537}'),(151,11,-1075.41,-253.424,37.7554,'{\"X\":-1075.2792,\"Y\":-253.33186,\"Z\":44.00659}'),(152,11,-1078.09,-254.756,37.7554,'{\"X\":-1077.9033,\"Y\":-254.7033,\"Z\":44.00659}'),(153,1,242.242,225.046,106.284,NULL),(154,1,247.556,223.187,106.284,NULL),(155,1,252.659,221.393,106.284,NULL),(159,4,-538.088,-892.101,24.5956,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":0.0}'),(160,4,-583.701,-930.752,36.8286,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":1.5336909}'),(161,2,-1222.62,-906.923,12.3121,NULL),(162,7,-814.259,-183.956,37.5531,NULL),(163,8,-541.371,-192.659,47.4103,NULL),(164,2,1393.32,3604.88,34.9751,NULL),(165,4,-1462.39,-2679.02,13.9297,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":1.7315865}'),(171,4,1107.02,99.9824,80.874,'{\"Roll\":0.0,\"Pitch\":0.0,\"Yaw\":0.8905303}');
/*!40000 ALTER TABLE `pontos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `precos`
--

DROP TABLE IF EXISTS `precos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `precos` (
  `Tipo` int NOT NULL,
  `Nome` varchar(25) COLLATE utf8mb4_general_ci NOT NULL,
  `Valor` int DEFAULT '0',
  PRIMARY KEY (`Tipo`,`Nome`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `precos`
--

LOCK TABLES `precos` WRITE;
/*!40000 ALTER TABLE `precos` DISABLE KEYS */;
INSERT INTO `precos` VALUES (1,'adder',1800000),(1,'autarch',1750000),(1,'banshee',65000),(1,'bestiagts',85000),(1,'bullet',425000),(1,'carbonizzare',85000),(1,'cheetah',850000),(1,'cheetah2',325000),(1,'comet2',350000),(1,'comet3',375000),(1,'comet5',435000),(1,'coquette',145000),(1,'coquette2',225000),(1,'coquette3',28000),(1,'cyclone',600000),(1,'deveste',650000),(1,'drafter',55000),(1,'emerus',1375000),(1,'entity2',1850000),(1,'entityxf',1650000),(1,'f620',120000),(1,'feltzer2',65000),(1,'fmj',2100000),(1,'furia',1975000),(1,'furoregt',110000),(1,'gb200',180000),(1,'gp1',975000),(1,'imorgon',130000),(1,'infernus',1100000),(1,'infernus2',450000),(1,'issi7',210000),(1,'italigtb',2200000),(1,'italigtb2',2500000),(1,'italigto',550000),(1,'jackal',80000),(1,'jugular',230000),(1,'khamelion',230000),(1,'locust',320000),(1,'lynx',180000),(1,'mamba',175000),(1,'monroe',225000),(1,'neo',650000),(1,'neon',870000),(1,'nero',2800000),(1,'nero2',3100000),(1,'ninef',300000),(1,'ninef2',330000),(1,'osiris',2700000),(1,'pariah',160000),(1,'penetrator',1650000),(1,'pfister811',1975000),(1,'prototipo',3500000),(1,'raiden',145000),(1,'rapidgt3',28500),(1,'reaper',2375000),(1,'revolter',136000),(1,'ruston',400000),(1,'sc1',1325000),(1,'sheava',1800000),(1,'stinger',75000),(1,'stingergt',135000),(1,'stromberg',225000),(1,'sultanrs',210000),(1,'swinger',325000),(1,'t20',2750000),(1,'taipan',1350000),(1,'tempesta',2450000),(1,'tezeract',4500000),(1,'thrax',2650000),(1,'torero',325000),(1,'tropos',315000),(1,'turismo2',435000),(1,'turismor',2100000),(1,'tyrant',2850000),(1,'tyrus',1875000),(1,'vacca',2350000),(1,'vagner',3250000),(1,'verlierer2',190000),(1,'viseris',380000),(1,'visione',3600000),(1,'voltic',1350000),(1,'xa21',2500000),(1,'zentorno',3750000),(1,'zorrusso',2900000),(1,'ztype',500000),(2,'Bola de Baseball',2),(2,'Celular',500),(2,'Chave de Grifo',1300),(2,'Galão de Combustível',1500),(2,'Garrafa',50),(2,'Martelo',650),(2,'Máscara',500),(2,'Pé de Cabra',450),(2,'Peça Veicular',25),(2,'Rádio Comunicador',150),(2,'Soco Inglês',800),(2,'Taco de Baseball',25),(2,'Taco de Golfe',500),(3,'dinghy',130000),(3,'dinghy2',120000),(3,'jetmax',250000),(3,'marquis',230000),(3,'seashark',55000),(3,'seashark3',60000),(3,'speeder',210000),(3,'squalo',310000),(3,'suntrap',200000),(3,'Toro',250000),(3,'tropic',190000),(3,'tropic2',215000),(3,'tug',80000),(4,'cargobob2',1200000),(4,'havok',1100000),(4,'Maverick',1850000),(4,'seasparrow',1350000),(4,'supervolito',2500000),(4,'supervolito2',2800000),(4,'swift',3000000),(4,'swift2',3200000),(4,'volatus',3500000),(5,'airbus',35000),(5,'benson',17000),(5,'biff',7000),(5,'boxville',22000),(5,'boxville2',22000),(5,'boxville3',22000),(5,'boxville4',22000),(5,'bulldozer',65000),(5,'burrito',17000),(5,'burrito2',17000),(5,'burrito3',19500),(5,'burrito4',17000),(5,'bus',35000),(5,'camper',27000),(5,'coach',40000),(5,'dloader',6500),(5,'flatbed',21000),(5,'gburrito',23000),(5,'gburrito2',23000),(5,'hauler',15000),(5,'Mixer',14000),(5,'mixer2',14500),(5,'mower',5500),(5,'mule',16000),(5,'mule2',16000),(5,'mule3',16000),(5,'mule4',16000),(5,'packer',25000),(5,'paradise',17000),(5,'phantom',27000),(5,'phantom3',40000),(5,'pony',16000),(5,'pony2',16000),(5,'pounder',17000),(5,'pounder2',17000),(5,'rentalbus',13000),(5,'rubble',12000),(5,'rumpo',16500),(5,'rumpo2',16500),(5,'rumpo3',35000),(5,'scrap',13500),(5,'speedo',20000),(5,'speedo2',15550),(5,'speedo4',21000),(5,'taco',9000),(5,'Taxi',11500),(5,'tiptruck',14500),(5,'tourbus',18000),(5,'towtruck',19500),(5,'towtruck2',14500),(5,'tractor2',25000),(5,'trash',8000),(5,'trash2',7500),(5,'utillitruck',11500),(5,'utillitruck2',10500),(5,'utillitruck3',16000),(5,'youga',14500),(5,'youga2',16000),(6,'alphaz1',2800000),(6,'cuban800',1900000),(6,'dodo',2300000),(6,'duster',1500000),(6,'howard',3800000),(6,'luxor',5500000),(6,'mammatus',2200000),(6,'nimbus',4500000),(6,'rogue',2600000),(6,'seabreeze',1800000),(6,'Shamal',4200000),(6,'stunt',1800000),(6,'titan',2500000),(6,'velum',2100000),(6,'Vestra',3100000),(7,'advancedrifle',18000),(7,'appistol',12000),(7,'assaultrifle',13000),(7,'assaultriflemkii',15000),(7,'assaultshotgun',35000),(7,'assaultsmg',13000),(7,'battleaxe',500),(7,'bullpuprifle',18000),(7,'bullpupriflemkii',21000),(7,'bullpupshotgun',13000),(7,'carbinerifle',13500),(7,'carbineriflemkii',17000),(7,'ceramicpistol',2000),(7,'combatmg',13000),(7,'combatmgmkii',15000),(7,'combatpdw',10000),(7,'combatpistol',4500),(7,'compactrifle',35000),(7,'dagger',2500),(7,'doubleaction',3500),(7,'DoubleActionRevolver',36800),(7,'DoubleBarrelShotgun',7000),(7,'granade',40000),(7,'GusenbergSweeper',9000),(7,'hatchet',500),(7,'heavypistol',5000),(7,'HeavyRevolver',3400),(7,'HeavyRevolverMkII',4800),(7,'heavyshotgun',33000),(7,'heavysniper',60000),(7,'heavysnipermkii',75000),(7,'knife',3000),(7,'machete',1500),(7,'machinepistol',8000),(7,'marksmanpistol',1800),(7,'marksmanrifle',50000),(7,'marksmanriflemkii',55000),(7,'mg',10500),(7,'microsmg',17000),(7,'molotov',5000),(7,'musket',2000),(7,'navyrevolver',1300),(7,'pipebombs',85000),(7,'pistol',2000),(7,'pistol50',4000),(7,'PistolMkII',2950),(7,'ProximityMines',100000),(7,'pumpshotgun',7000),(7,'pumpshotgunmkii',10000),(7,'SawedOffShotgun',7000),(7,'smg',10500),(7,'smgmkii',13000),(7,'sniperrifle',45000),(7,'snspistol',2300),(7,'SNSPistolMkII',2800),(7,'specialcarbine',18000),(7,'specialcarbinemkii',20000),(7,'stickybomb',80000),(7,'stone_hatchet',1000),(7,'SweeperShotgun',33000),(7,'switchblade',1000),(7,'vintagepistol',2500),(8,'lixeiro',170),(8,'mecanico',350),(8,'taxista',400),(9,'bmx',600),(9,'cruiser',300),(9,'fixter',800),(9,'scorcher',1200),(9,'tribike',1400),(9,'tribike2',1500),(9,'tribike3',1600),(10,'akuma',55000),(10,'avarus',12000),(10,'bagger',8500),(10,'bati',45000),(10,'bf400',110000),(10,'blazer',15000),(10,'carbonrs',25000),(10,'chimera',18000),(10,'cliffhanger',25000),(10,'daemon',9500),(10,'daemon2',10500),(10,'defiler',35000),(10,'diablous',45000),(10,'diablous2',53000),(10,'double',49000),(10,'enduro',13000),(10,'esskey',9000),(10,'faggio',7500),(10,'faggio2',5000),(10,'faggio3',4500),(10,'fcr',18000),(10,'fcr2',33000),(10,'hakuchou',65000),(10,'hakuchou2',75000),(10,'hexer',16000),(10,'lectro',14500),(10,'manchez',19500),(10,'nemesis',16500),(10,'nightblade',25000),(10,'pcj',13500),(10,'ratbike',4500),(10,'ruffian',18500),(10,'sanchez',18500),(10,'sanchez2',18500),(10,'sovereign',11000),(10,'thrust',22000),(10,'vader',17900),(10,'vindicator',25000),(10,'vortex',21500),(10,'wolfsbane',22000),(10,'zombiea',11000),(10,'zombieb',13500),(11,'banshee2',475000),(11,'bison',23000),(11,'bison2',19000),(11,'bison3',21000),(11,'brawler',32000),(11,'buffalo',45000),(11,'buffalo2',50000),(11,'faction',14000),(11,'faction2',20000),(11,'faction3',35000),(11,'fugitive',18900),(11,'gauntlet',21500),(11,'gauntlet3',19500),(11,'gauntlet4',65000),(11,'gresley',33000),(11,'imperator',18500),(11,'journey',7000),(11,'landstalker',36500),(11,'landstalker2',55000),(11,'picador',12000),(11,'ratloader',4500),(11,'ratloader2',11000),(11,'regina',6500),(11,'stalion',16000),(11,'stratum',17500),(11,'stretch',80000),(11,'surge',28500),(12,'asterope',22000),(12,'bjxl',31000),(12,'blista',9000),(12,'blista2',13500),(12,'cheburek',14750),(12,'dilettante',6500),(12,'elegy',70000),(12,'elegy2',85000),(12,'everon',40000),(12,'fq2',32500),(12,'futo',18000),(12,'habanero',19500),(12,'hellion',18000),(12,'intruder',18000),(12,'jester',210000),(12,'jester3',190000),(12,'kanjo',17500),(12,'kuruma',85000),(12,'penumbra',72000),(12,'prairie',9000),(12,'rebel',13000),(12,'rebel2',15500),(12,'savestra',21000),(12,'sugoi',125000),(12,'sultan',41000),(12,'sultan2',33500),(12,'z190',195000),(13,'asbo',7000),(13,'bfinjection',13000),(13,'bifta',35000),(13,'brioso',8500),(13,'casco',55000),(13,'Club',16000),(13,'cog55',35000),(13,'cogcabrio',45000),(13,'cognoscenti',37000),(13,'dubsta',49000),(13,'dubsta2',52000),(13,'dubsta3',135000),(13,'dynasty',8800),(13,'exemplar',48000),(13,'fagaloa',11700),(13,'felon',60000),(13,'felon2',75000),(13,'feltzer3',52100),(13,'glendale',12500),(13,'glendale2',15000),(13,'gt500',23000),(13,'huntley',41000),(13,'ingot',11500),(13,'issi2',13000),(13,'issi3',7250),(13,'jb700',155000),(13,'komoda',90000),(13,'krieger',1780000),(13,'massacro',160000),(13,'michelli',8520),(13,'nebula',10500),(13,'novak',48000),(13,'oracle',35000),(13,'oracle2',65000),(13,'panto',5500),(13,'paragon',280000),(13,'pigalle',12750),(13,'rapidgt',125000),(13,'rapidgt3',43000),(13,'rebla',43000),(13,'rocoto',46000),(13,'schafter3',38000),(13,'schlagen',380000),(13,'schwarzer',48000),(13,'sentinel',38000),(13,'sentinel3',29500),(13,'serrano',26000),(13,'seven70',280000),(13,'shafter2',26000),(13,'specter',310000),(13,'stafford',41000),(13,'superd',50000),(13,'surano',155000),(13,'surfer',13000),(13,'surfer2',9500),(13,'tailgater',32500),(13,'toros',85000),(13,'warrener',12750),(13,'windsor',90000),(13,'windsor2',120000),(13,'xls',65000),(13,'zion',28000),(13,'zion2',32000),(13,'zion3',35000),(14,'alpha',21600),(14,'asea',12000),(14,'bodhi2',10000),(14,'brutus',28000),(14,'btype',850000),(14,'btype2',725000),(14,'btype3',775000),(14,'buccaneer',15500),(14,'buccaneer2',22500),(14,'cavalcade',26000),(14,'cavalcade2',32000),(14,'emperor',7500),(14,'emperor2',5500),(14,'granger',29500),(14,'hermes',7500),(14,'impaler',25000),(14,'kalahari',11000),(14,'kamacho',37000),(14,'manana',6250),(14,'manana2',11000),(14,'mesa',40000),(14,'mesa3',43000),(14,'moonbeam',8750),(14,'moonbeam2',13750),(14,'premier',21500),(14,'primo',14500),(14,'primo2',19000),(14,'rancherxl',16500),(14,'rhapsody',9800),(14,'sabregt',23000),(14,'sabregt2',27000),(14,'seminole',22500),(14,'seminole2',16000),(14,'tampa',13000),(14,'tornado',9750),(14,'tornado2',12250),(14,'tornado3',4500),(14,'tornado4',3500),(14,'tornado5',13000),(14,'tulip',26000),(14,'vamos',32000),(14,'vigero',17000),(14,'virgo',11000),(14,'virgo2',17000),(14,'virgo3',13000),(14,'voodoo',7000),(14,'voodoo2',3500),(14,'vstr',91000),(14,'washington',18500),(14,'yosemite',13500),(14,'yosemite2',22000),(15,'baller',28000),(15,'baller2',65000),(15,'blade',17500),(15,'bobcatxl',16700),(15,'caracara2',33000),(15,'chino',9800),(15,'chino2',14700),(15,'clique',24000),(15,'contender',39000),(15,'deluxo',35000),(15,'deviant',19000),(15,'dominator',40000),(15,'dominator3',75000),(15,'dukes',29000),(15,'ellie',32000),(15,'flashgt',88000),(15,'fusilade',38000),(15,'guardian',55000),(15,'hotknife',90000),(15,'hustler',18000),(15,'minivan',7000),(15,'minivan2',13000),(15,'nightshade',29000),(15,'patriot',41500),(15,'patriot2',71000),(15,'peyote',18000),(15,'phoenix',16700),(15,'radi',27500),(15,'retinue',13000),(15,'retinue2',22600),(15,'riata',22500),(15,'ruiner',13000),(15,'sadler',11500),(15,'sandking',35000),(15,'sandking2',32500),(15,'slamvan',20000),(15,'slamvan2',22000),(15,'slamvan3',25000),(15,'stanier',29000),(16,'lixeiro',150),(16,'mecanico',50),(16,'taxista',100);
/*!40000 ALTER TABLE `precos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prisoes`
--

DROP TABLE IF EXISTS `prisoes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prisoes` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Data` datetime DEFAULT NULL,
  `Preso` int DEFAULT '0',
  `Policial` int DEFAULT '0',
  `Termino` datetime NOT NULL,
  `Descricao` text COLLATE utf8mb4_general_ci NOT NULL,
  `Crimes` text COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prisoes`
--

LOCK TABLES `prisoes` WRITE;
/*!40000 ALTER TABLE `prisoes` DISABLE KEYS */;
/*!40000 ALTER TABLE `prisoes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `procurados`
--

DROP TABLE IF EXISTS `procurados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `procurados` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Data` datetime NOT NULL,
  `PersonagemPolicial` int NOT NULL DEFAULT '0',
  `Personagem` int NOT NULL DEFAULT '0',
  `Veiculo` int NOT NULL DEFAULT '0',
  `Motivo` text COLLATE utf8mb4_general_ci NOT NULL,
  `DataExclusao` datetime DEFAULT NULL,
  `PersonagemPolicialExclusao` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `procurados`
--

LOCK TABLES `procurados` WRITE;
/*!40000 ALTER TABLE `procurados` DISABLE KEYS */;
/*!40000 ALTER TABLE `procurados` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propriedades`
--

DROP TABLE IF EXISTS `propriedades`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propriedades` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Interior` int NOT NULL DEFAULT '0',
  `Valor` int DEFAULT '0',
  `Personagem` int DEFAULT '0',
  `EntradaPosX` float DEFAULT '0',
  `EntradaPosY` float DEFAULT '0',
  `EntradaPosZ` float DEFAULT '0',
  `SaidaPosX` float DEFAULT '0',
  `SaidaPosY` float DEFAULT '0',
  `SaidaPosZ` float DEFAULT '0',
  `Dimensao` bigint NOT NULL DEFAULT '0',
  `Endereco` varchar(255) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propriedades`
--

LOCK TABLES `propriedades` WRITE;
/*!40000 ALTER TABLE `propriedades` DISABLE KEYS */;
/*!40000 ALTER TABLE `propriedades` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `punicoes`
--

DROP TABLE IF EXISTS `punicoes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `punicoes` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Tipo` int DEFAULT '0',
  `Duracao` int DEFAULT '0',
  `Data` datetime DEFAULT NULL,
  `Personagem` int DEFAULT '0',
  `Motivo` varchar(255) COLLATE utf8mb4_general_ci DEFAULT '',
  `Usuariostaff` int DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `punicoes`
--

LOCK TABLES `punicoes` WRITE;
/*!40000 ALTER TABLE `punicoes` DISABLE KEYS */;
/*!40000 ALTER TABLE `punicoes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ranks`
--

DROP TABLE IF EXISTS `ranks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ranks` (
  `Faccao` int NOT NULL,
  `Codigo` int NOT NULL,
  `Nome` varchar(25) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Salario` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Faccao`,`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ranks`
--

LOCK TABLES `ranks` WRITE;
/*!40000 ALTER TABLE `ranks` DISABLE KEYS */;
INSERT INTO `ranks` VALUES (1,1,'Police Officer I',450),(1,2,'Police Officer II',470),(1,3,'Police Officer III',500),(1,4,'Police Officer III+1',550),(1,5,'Police Detective I',550),(1,6,'Police Detective II',650),(1,7,'Police Detective III',800),(1,8,'Police Sergeant I',800),(1,9,'Police Sergeant II',850),(1,10,'Police Lieutenant I',1000),(1,11,'Police Lieutenant II',1050),(1,12,'Police Captain I',1200),(1,13,'Police Captain II',1350),(1,14,'Police Captain III',1400),(1,15,'Police Commander',1600),(1,16,'Deputy Chief of Police',1750),(1,17,'Assistant Chief of Police',1900),(1,18,'Chief of Police',2000),(2,1,'Fire Recruit',450),(2,2,'Firefighter I',480),(2,3,'Firefighter II',470),(2,4,'Firefighter III',530),(2,5,'Fire Engineer',550),(2,6,'Fire Lieutenant I',650),(2,7,'Fire Lieutenant II',700),(2,8,'Fire Captain I',750),(2,9,'Fire Captain II',850),(2,10,'Fire Battalion Chief',1000),(2,11,'Assistant Fire Chief',1150),(2,12,'Deputy Fire Chief',1350),(2,13,'Fire Chief',1600),(3,1,'Trainee',1),(3,2,'Employee',1),(3,4,'Technician',1),(3,5,'Security Detail',1),(3,6,'LSDOT Employee I',1),(3,7,'LSDOT Employee II',1),(3,8,'LSDOT Employee III',1),(3,9,'LSDOT Supervisor',1),(3,10,'HR Agent I',1),(3,11,'HR Agent II',1),(3,12,'HR Supervisor',1),(3,13,'Social Services Agent',1),(3,14,'Press Official',1),(3,15,'Licensing Official',1),(3,16,'Medical Examiner',1),(3,17,'Psychologist',1),(3,18,'Budget Analyst',1),(3,19,'Assit. Outreach Manager',1),(3,20,'e-Procurement Manager',1),(3,21,'Outreach Manager',1),(3,22,'Quality Control Manager',1),(3,23,'Department Assistant',1),(3,24,'Department Director',1),(3,25,'Office Staff Assistant',1),(3,26,'City Attorney',1),(3,27,'Press Secretary',1),(3,28,'Deputy Mayor',1),(3,29,'Deputy Chief of Staff',1),(3,30,'Chief of Staff',1),(3,31,'Mayor',1),(4,1,'Employee 1',430),(4,2,'Employee 2',460),(4,3,'Employee 3',490),(4,4,'Employee 4',510),(4,5,'Employee 5',530),(4,6,'Employee 6',560),(4,7,'Employee 7',590),(4,8,'Employee 8',610),(4,9,'Management Leader',650),(4,10,'Management Producer',680),(4,11,'Management Officer',690),(4,12,'Management Officer Staff',710),(4,13,'Chief Operations Officer',750),(4,14,'Chief Executive Officer',780),(4,15,'Vice President',810),(4,16,'President',900),(5,1,'Gostoso',3000);
/*!40000 ALTER TABLE `ranks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `respostas`
--

DROP TABLE IF EXISTS `respostas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `respostas` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(255) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Pergunta` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `respostas`
--

LOCK TABLES `respostas` WRITE;
/*!40000 ALTER TABLE `respostas` DISABLE KEYS */;
INSERT INTO `respostas` VALUES (1,'Paro o meu carro no lado incorreto de tráfego e observo a perseguição.',1),(2,'Encosto o meu carro no canto da rua e observo a perseguição.',1),(3,'Não faço nada e fico parado no meio da rua.',1),(4,'Metagaming.',5),(5,'Cortesia Comum.',5),(6,'Regra de Comunicações.',5),(7,'Regras para Facções Legais.',2),(8,'Regras de Empresas.',2),(9,'Desenvolvimento.',2),(11,'Desenvolvimento.',3),(12,'Abuso das Mecânicas do GTA.',3),(13,'Powergaming.',3),(14,'Conversar com o jogador via /b e explicar para ele que o que está fazendo é errado, para só depois se for o caso abrir uma denúncia. ',4),(15,'Conversar com o jogador via /pm e explicar para ele que o que está fazendo é errado, para só depois se for o caso abrir uma denúncia. ',4),(16,'Denunciar o jogador assim que notar a quebra de regra.',4),(17,'Powergaming.',6),(18,'Metagaming.',6),(19,'Cortesia Comum.',6),(20,'Significam que o texto está destacado.',7),(21,'Significam que o texto está em OOC.',7),(22,'Significam que o texto está em IC.',7),(23,'Segue o desenvolvimento daquela interpretação normalmente.',8),(24,'Recorre ao fórum e abre uma denúncia contra o jogador.',8),(25,'Reclama por um canal Out of Character com o outro jogador em questão.',8),(26,'Não há problema nenhum em matar um personagem caso você possua motivos suficientes OOC.',9),(27,'Não há problema nenhum em jogar com um amigo utilizando um programa de voz para combinarem ações, falas e Pontos de encontro. (Discord, Skype, Whatsapp)',9),(28,'Não há problema nenhum em fazer parte de um assalto à mão armada estando com um capacete, uma balaclava ou outro item que te deixe mascarado.',9),(29,'Metagaming e Cortesia Comum.',10),(30,'Abuso da física e Cortesia Comum.',10),(31,'Powergaming e Cortesia Comum. ',10),(32,'Drive-by.',12),(33,'Deathmatch.',12),(34,'Metagaming.',12),(35,'Mandar uma /PM no jogo para o membro ou líder da facção e pedir para se juntar a ela.',13),(36,'Frequentar lugares que a facção pertence ou morar no mesmo bairro (em caso de gangues), ganhando a confiança dos membros enquanto se aproxima de maneira In Character.',13),(37,'Enviar uma mensagem privada para o líder no fórum, aceitando o Contrato de CK para participar da facção e postar screenshots em seu tópico.',13),(38,'Você é um civil que pretende ter uma diversão a mais no seu jogo ou ainda não participa de uma facção criminosa, por isso resolve comprar uma arma. Para comprar, você para em algum bairro de gangster e pergunta se eles têm um cano para vender.',14),(39,'Você é membro de uma facção criminosa e tem desenvolvido seu roleplay criminoso desde o início, porém ainda não teve oportunidade de possuir uma arma. Você busca uma arma através de outros criminosos, até da mesma facção, de forma In Character.',14),(40,'Você é um policial e quer carregar uma arma off duty, ou seja, fora do serviço. Mediante a alguma autorização, você carrega uma arma livremente.',14);
/*!40000 ALTER TABLE `respostas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `soss`
--

DROP TABLE IF EXISTS `soss`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `soss` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Data` datetime DEFAULT NULL,
  `Mensagem` varchar(255) COLLATE utf8mb4_general_ci DEFAULT '',
  `Usuario` int NOT NULL DEFAULT '0',
  `DataResposta` datetime DEFAULT NULL,
  `Usuariostaff` int DEFAULT '0',
  `TipoResposta` int DEFAULT '0',
  `MotivoRejeicao` varchar(255) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `soss`
--

LOCK TABLES `soss` WRITE;
/*!40000 ALTER TABLE `soss` DISABLE KEYS */;
/*!40000 ALTER TABLE `soss` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(25) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Email` varchar(100) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `Senha` varchar(128) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `SocialClubRegistro` bigint DEFAULT '0',
  `IPRegistro` varchar(25) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `DataRegistro` datetime NOT NULL,
  `IPUltimoAcesso` varchar(25) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `DataUltimoAcesso` datetime NOT NULL,
  `Staff` int NOT NULL DEFAULT '0',
  `SocialClubUltimoAcesso` bigint DEFAULT '0',
  `PossuiNamechange` bit(1) NOT NULL DEFAULT b'0',
  `QuantidadeSOSAceitos` int NOT NULL DEFAULT '0',
  `TempoTrabalhoAdministrativo` int NOT NULL DEFAULT '0',
  `TimeStamp` bit(1) DEFAULT b'0',
  `HardwareIdHashRegistro` bigint NOT NULL DEFAULT '0',
  `HardwareIdExHashRegistro` bigint NOT NULL DEFAULT '0',
  `HardwareIdHashUltimoAcesso` bigint NOT NULL DEFAULT '0',
  `HardwareIdExHashUltimoAcesso` bigint NOT NULL DEFAULT '0',
  `TokenConfirmacao` varchar(6) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `VIP` int NOT NULL DEFAULT '0',
  `DataExpiracaoVIP` datetime DEFAULT NULL,
  `PossuiNamechangeForum` bit(1) NOT NULL DEFAULT b'0',
  `TogPM` bit(1) NOT NULL DEFAULT b'0',
  `TogChatStaff` bit(1) NOT NULL DEFAULT b'0',
  `TogChatFaccao` bit(1) NOT NULL DEFAULT b'0',
  `PossuiPlateChange` bit(1) NOT NULL DEFAULT b'0',
  `TogAnuncio` int NOT NULL DEFAULT '0',
  `ContentCreator` bit(1) DEFAULT b'0',
  `Discord` bigint DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `veiculos`
--

DROP TABLE IF EXISTS `veiculos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `veiculos` (
  `Codigo` int NOT NULL AUTO_INCREMENT,
  `Modelo` varchar(25) COLLATE utf8mb4_general_ci DEFAULT '',
  `PosX` float DEFAULT '0',
  `PosY` float DEFAULT '0',
  `PosZ` float DEFAULT '0',
  `RotX` float DEFAULT '0',
  `RotY` float DEFAULT '0',
  `RotZ` float DEFAULT '0',
  `Cor1R` int DEFAULT '0',
  `Cor1G` int DEFAULT '0',
  `Cor1B` int NOT NULL DEFAULT '0',
  `Cor2R` int NOT NULL DEFAULT '0',
  `Cor2G` int NOT NULL DEFAULT '0',
  `Cor2B` int NOT NULL DEFAULT '0',
  `Personagem` int DEFAULT '0',
  `Placa` varchar(8) COLLATE utf8mb4_general_ci DEFAULT '',
  `Faccao` int DEFAULT '0',
  `EngineHealth` int NOT NULL DEFAULT '1000',
  `Livery` int NOT NULL DEFAULT '0',
  `ValorApreensao` int DEFAULT '0',
  `Combustivel` int NOT NULL DEFAULT '0',
  `Danos` text COLLATE utf8mb4_general_ci NOT NULL,
  `Estacionou` bit(1) NOT NULL DEFAULT b'0',
  `VendidoFerroVelho` bit(1) NOT NULL DEFAULT b'0',
  `Emprego` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `veiculos`
--

LOCK TABLES `veiculos` WRITE;
/*!40000 ALTER TABLE `veiculos` DISABLE KEYS */;
/*!40000 ALTER TABLE `veiculos` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-02-21 10:11:01

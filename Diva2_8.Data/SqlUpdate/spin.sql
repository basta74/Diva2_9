-- --------------------------------------------------------
-- Hostitel:                     127.0.0.1
-- Verze serveru:                10.2.6-MariaDB - mariadb.org binary distribution
-- OS serveru:                   Win64
-- HeidiSQL Verze:               10.3.0.5807
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Exportování struktury pro tabulka diva2_akt.category_8
CREATE TABLE IF NOT EXISTS `category_8` (
  `CAT_ID` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `CAT_NAZEV` varchar(30) COLLATE utf8_czech_ci DEFAULT NULL,
  `CAT_PARENT` int(4) unsigned NOT NULL DEFAULT 0,
  `CAT_CHILD` int(1) NOT NULL DEFAULT 0,
  `CAT_PATH` char(50) COLLATE utf8_czech_ci NOT NULL,
  `CAT_VISIBLE` int(1) unsigned DEFAULT 0,
  `CAT_EDIT` int(2) DEFAULT 0,
  `CAT_PORADI` int(2) DEFAULT 0,
  PRIMARY KEY (`CAT_ID`),
  UNIQUE KEY `id_cat` (`CAT_ID`,`CAT_PARENT`)
) ENGINE=MyISAM AUTO_INCREMENT=101 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.clanky
CREATE TABLE IF NOT EXISTS `clanky` (
  `CLA_ID` int(6) NOT NULL AUTO_INCREMENT,
  `CLA_CAT_ID` int(6) NOT NULL DEFAULT 0,
  `CLA_PARENT` int(11) DEFAULT NULL,
  `CLA_ROK` int(4) NOT NULL DEFAULT 0,
  `CLA_ROCNIK` varchar(7) COLLATE utf8_czech_ci DEFAULT NULL,
  `CLA_NADPIS` varchar(255) COLLATE utf8_czech_ci NOT NULL,
  `CLA_POPIS` mediumtext COLLATE utf8_czech_ci NOT NULL,
  `CLA_OBSAH` longtext COLLATE utf8_czech_ci NOT NULL,
  `CLA_DATUM` date NOT NULL DEFAULT '0000-00-00',
  `CLA_AUTOR` int(8) NOT NULL DEFAULT 0,
  `CLA_PIC` varchar(100) COLLATE utf8_czech_ci DEFAULT NULL,
  `CLA_PIC_SUM` int(2) DEFAULT NULL,
  `CLA_DISKUSE` int(1) NOT NULL DEFAULT 0,
  `CLA_VISIBLE` int(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`CLA_ID`)
) ENGINE=MyISAM AUTO_INCREMENT=74 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.listy
CREATE TABLE IF NOT EXISTS `listy` (
  `PL_ID` int(6) NOT NULL,
  `PL_NAZEV` varchar(50) CHARACTER SET cp1250 COLLATE cp1250_czech_cs NOT NULL,
  `PL_CESTA` varchar(100) CHARACTER SET cp1250 COLLATE cp1250_czech_cs NOT NULL,
  `PL_PRAVA` varchar(50) CHARACTER SET cp1250 COLLATE cp1250_czech_cs NOT NULL,
  `PL_PORADI` int(11) DEFAULT NULL,
  `PL_VISIBLE` smallint(6) NOT NULL DEFAULT 0,
  KEY `PL_ID` (`PL_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.pobocka_for_user
CREATE TABLE IF NOT EXISTS `pobocka_for_user` (
  `PU_USER_ID` int(11) NOT NULL,
  `PU_POB_ID` int(11) NOT NULL,
  KEY `PU_USER_ID` (`PU_USER_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.prava
CREATE TABLE IF NOT EXISTS `prava` (
  `PRA_ID` int(11) NOT NULL AUTO_INCREMENT,
  `PRA_NAZEV` varchar(255) CHARACTER SET cp1250 COLLATE cp1250_czech_cs NOT NULL,
  `PRA_POPIS` varchar(255) CHARACTER SET cp1250 COLLATE cp1250_czech_cs NOT NULL,
  `PRA_ABLE` int(1) NOT NULL,
  PRIMARY KEY (`PRA_ID`)
) ENGINE=MyISAM AUTO_INCREMENT=65 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.prava_for_role
CREATE TABLE IF NOT EXISTS `prava_for_role` (
  `ROLE_ID` int(11) NOT NULL,
  `PRAVA_ID` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.prava_for_user
CREATE TABLE IF NOT EXISTS `prava_for_user` (
  `USER_ID` int(11) NOT NULL,
  `PRAVA_ID` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.prijmove_doklady
CREATE TABLE IF NOT EXISTS `prijmove_doklady` (
  `rok` smallint(5) unsigned DEFAULT NULL,
  `poradi` int(10) unsigned DEFAULT NULL,
  KEY `Index 1` (`rok`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci COMMENT='ciselnik prijmovych dokladu';

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.role
CREATE TABLE IF NOT EXISTS `role` (
  `ROLE_ID` int(6) NOT NULL AUTO_INCREMENT,
  `ROLE_NAZEV` varchar(255) CHARACTER SET cp1250 COLLATE cp1250_czech_cs NOT NULL,
  `ROLE_POPIS` varchar(255) CHARACTER SET cp1250 COLLATE cp1250_czech_cs NOT NULL,
  `ROLE_ABLE` int(1) NOT NULL,
  PRIMARY KEY (`ROLE_ID`)
) ENGINE=MyISAM AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.role_for_user
CREATE TABLE IF NOT EXISTS `role_for_user` (
  `RU_ROLE_ID` int(11) NOT NULL,
  `RU_USER_ID` int(11) NOT NULL,
  KEY `role_id` (`RU_ROLE_ID`),
  KEY `user_id` (`RU_USER_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.sms
CREATE TABLE IF NOT EXISTS `sms` (
  `ID` int(11) NOT NULL,
  `pobockaID` int(3) NOT NULL,
  `spinID` int(11) NOT NULL,
  `zakaznikID` int(11) NOT NULL,
  `userID` int(11) NOT NULL DEFAULT 0,
  `cislo` varchar(15) COLLATE cp1250_czech_cs NOT NULL,
  `email` varchar(70) COLLATE cp1250_czech_cs NOT NULL,
  `text` varchar(255) COLLATE cp1250_czech_cs NOT NULL,
  `createUnix` int(11) NOT NULL,
  `sendUnix` int(11) DEFAULT 0,
  `sendUserID` int(11) DEFAULT 0,
  `sendStav` int(255) DEFAULT -1,
  `vyrizeno` int(1) NOT NULL DEFAULT 0,
  UNIQUE KEY `ID` (`ID`),
  KEY `zakaznikID` (`zakaznikID`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1250 COLLATE=cp1250_czech_cs;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spinhod
CREATE TABLE IF NOT EXISTS `spinhod` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `verze` int(11) NOT NULL DEFAULT 0,
  `spinTID` int(11) NOT NULL DEFAULT 0,
  `pobocka` int(2) NOT NULL,
  `zdroj` int(11) NOT NULL DEFAULT 1,
  `rok` int(4) NOT NULL,
  `tyden` int(2) NOT NULL,
  `denVTydnu` int(1) NOT NULL,
  `datum` date DEFAULT NULL,
  `DUX` int(11) DEFAULT 0,
  `hodina` int(2) NOT NULL,
  `hodinaZacatek` time DEFAULT NULL,
  `nazev` varchar(255) CHARACTER SET utf8 DEFAULT ' ',
  `kredit` int(11) NOT NULL,
  `minuty` int(2) NOT NULL,
  `pocetJednotek` int(11) NOT NULL DEFAULT 1,
  `lektor` int(2) NOT NULL,
  `lektor2` int(11) DEFAULT 0,
  `typ` int(2) NOT NULL DEFAULT 0,
  `pocetM` int(2) NOT NULL DEFAULT 0,
  `pocetZ` int(2) DEFAULT 0,
  `pocNahr` smallint(5) unsigned NOT NULL DEFAULT 0,
  `zauctovano` int(1) DEFAULT 0,
  PRIMARY KEY (`ID`),
  KEY `datum` (`datum`),
  KEY `rok-tyden` (`rok`,`tyden`)
) ENGINE=MyISAM AUTO_INCREMENT=9999 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci ROW_FORMAT=DYNAMIC;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spinhod_sablona
CREATE TABLE IF NOT EXISTS `spinhod_sablona` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `aktivni` int(1) NOT NULL DEFAULT 0,
  `pobocka` int(2) NOT NULL,
  `zdroj` int(11) NOT NULL DEFAULT 1,
  `denVTydnu` int(1) NOT NULL,
  `hodina` int(2) NOT NULL,
  `hodinaZacatek` char(5) CHARACTER SET utf8 NOT NULL,
  `nazev` varchar(50) CHARACTER SET utf8 DEFAULT '',
  `kredit` int(11) NOT NULL,
  `minuty` int(2) NOT NULL,
  `lektor` int(2) NOT NULL,
  `lektor2` int(11) DEFAULT 0,
  `typ` int(2) NOT NULL DEFAULT 0,
  `pocetM` int(2) NOT NULL DEFAULT 0,
  `pocetNahr` smallint(5) unsigned NOT NULL DEFAULT 0,
  KEY `ID` (`ID`)
) ENGINE=MyISAM AUTO_INCREMENT=211 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spinhod_text
CREATE TABLE IF NOT EXISTS `spinhod_text` (
  `spin_id` int(10) unsigned NOT NULL,
  `text` text NOT NULL,
  `ts` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE current_timestamp(),
  PRIMARY KEY (`spin_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='komentare k hodine';

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spinhod_typ
CREATE TABLE IF NOT EXISTS `spinhod_typ` (
  `ID` int(11) NOT NULL,
  `pobockaID` int(11) NOT NULL,
  `nazev` varchar(50) COLLATE utf8_czech_ci NOT NULL,
  `zkratka` char(4) COLLATE utf8_czech_ci NOT NULL,
  `jednPocet` int(11) NOT NULL DEFAULT 1,
  `jednPocetDo` int(11) NOT NULL DEFAULT 0,
  `jednMinut` int(11) NOT NULL DEFAULT 0,
  `kredit` int(11) NOT NULL DEFAULT 0,
  `mista` int(11) NOT NULL DEFAULT 1,
  `popis` text COLLATE utf8_czech_ci NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spinlektori
CREATE TABLE IF NOT EXISTS `spinlektori` (
  `ID` int(3) NOT NULL,
  `nick` varchar(20) CHARACTER SET cp1250 COLLATE cp1250_bin NOT NULL,
  `jmeno` varchar(50) COLLATE utf8_czech_ci NOT NULL,
  `titul` varchar(255) CHARACTER SET cp1250 COLLATE cp1250_bin DEFAULT NULL,
  `kredity` int(11) DEFAULT NULL,
  `email` varchar(50) CHARACTER SET cp1250 COLLATE cp1250_bin DEFAULT NULL,
  `tel` varchar(18) COLLATE utf8_czech_ci DEFAULT NULL,
  `popis` mediumtext CHARACTER SET cp1250 COLLATE cp1250_bin DEFAULT NULL,
  `poradi` int(11) DEFAULT NULL,
  `platnost` int(1) NOT NULL DEFAULT 0,
  `viditelnost` tinyint(1) NOT NULL DEFAULT 1,
  `userID` int(6) DEFAULT 0,
  `koeficient` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spinuser
CREATE TABLE IF NOT EXISTS `spinuser` (
  `spinID` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `pobockaID` int(11) NOT NULL,
  `pokladnaID` int(2) NOT NULL,
  `poradi` int(11) NOT NULL,
  `kontCislo` int(11) NOT NULL DEFAULT 0,
  `aktivni` int(1) DEFAULT 0,
  `bylTam` int(1) DEFAULT 0,
  `nahradnik` int(1) DEFAULT 0,
  `premiera` tinyint(3) unsigned DEFAULT 0,
  `zbytekTyp` char(2) CHARACTER SET utf8 DEFAULT NULL,
  `zbytek` int(11) DEFAULT NULL,
  `zbyvaDni` int(11) NOT NULL,
  `doMzdy` int(1) NOT NULL DEFAULT 1,
  `unix` int(10) unsigned DEFAULT 0,
  KEY `spinID` (`spinID`),
  KEY `user_Id` (`userID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci ROW_FORMAT=DYNAMIC;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spinuserchange
CREATE TABLE IF NOT EXISTS `spinuserchange` (
  `provedlID` int(11) NOT NULL,
  `spinID` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `status` char(1) CHARACTER SET cp1250 NOT NULL,
  `timestamp` timestamp NOT NULL DEFAULT current_timestamp(),
  KEY `spinID` (`spinID`),
  KEY `user_Id` (`userID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci ROW_FORMAT=DYNAMIC;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spinuser_text
CREATE TABLE IF NOT EXISTS `spinuser_text` (
  `spinID` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `kontrolniCislo` int(11) NOT NULL,
  `text` text COLLATE cp1250_czech_cs NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=cp1250 COLLATE=cp1250_czech_cs;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_ini
CREATE TABLE IF NOT EXISTS `spin_ini` (
  `PobockaID` int(11) NOT NULL DEFAULT 1,
  `Nazev` char(50) COLLATE utf8_czech_ci NOT NULL,
  `Hodnota` char(50) COLLATE utf8_czech_ci NOT NULL,
  UNIQUE KEY `pobockaNazev` (`PobockaID`,`Nazev`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_ini_main
CREATE TABLE IF NOT EXISTS `spin_ini_main` (
  `klic` varchar(50) COLLATE utf8_czech_ci NOT NULL,
  `value` varchar(50) COLLATE utf8_czech_ci NOT NULL,
  PRIMARY KEY (`klic`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_ini_zacatek
CREATE TABLE IF NOT EXISTS `spin_ini_zacatek` (
  `Pobocka` int(11) NOT NULL,
  `Poradi` int(11) NOT NULL,
  `Zacatek` char(5) CHARACTER SET latin1 NOT NULL,
  UNIQUE KEY `pobPoradi` (`Pobocka`,`Poradi`),
  KEY `Pobocka` (`Pobocka`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_mzdy
CREATE TABLE IF NOT EXISTS `spin_mzdy` (
  `pokladnaID` int(11) NOT NULL,
  `mesic` int(11) NOT NULL,
  `den` int(11) NOT NULL,
  `hodina` int(11) NOT NULL,
  `zakaznik` int(11) NOT NULL,
  `pro100` int(11) NOT NULL DEFAULT 0 COMMENT 'pln? hodina',
  `zak01` int(11) NOT NULL DEFAULT 0,
  `zak02` int(11) NOT NULL DEFAULT 0,
  `zak03` int(11) NOT NULL DEFAULT 0,
  `zak04` int(11) NOT NULL DEFAULT 0,
  `zak05` int(11) NOT NULL DEFAULT 0,
  `zak06` int(11) NOT NULL DEFAULT 0,
  `zak07` int(11) NOT NULL DEFAULT 0,
  `zak08` int(11) NOT NULL DEFAULT 0,
  `zak09` int(11) NOT NULL DEFAULT 0,
  `zak10` int(11) NOT NULL DEFAULT 0,
  `zak11` int(11) NOT NULL DEFAULT 0,
  `zak12` int(11) NOT NULL DEFAULT 0,
  `zak13` int(11) NOT NULL DEFAULT 0,
  `zak14` int(11) NOT NULL DEFAULT 0,
  `zak15` int(11) NOT NULL DEFAULT 0,
  `zak16` int(11) NOT NULL DEFAULT 0,
  `zak17` int(11) NOT NULL DEFAULT 0,
  `zak18` int(11) NOT NULL DEFAULT 0,
  `zak19` int(11) NOT NULL DEFAULT 0,
  `zak20` int(11) NOT NULL DEFAULT 0
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_platba_cas
CREATE TABLE IF NOT EXISTS `spin_platba_cas` (
  `ID` int(5) NOT NULL AUTO_INCREMENT,
  `verze` char(2) CHARACTER SET utf8 DEFAULT NULL,
  `kategorie` int(1) NOT NULL,
  `platba` int(8) NOT NULL,
  `popis` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `platnost` int(1) DEFAULT 0,
  `visible` int(1) NOT NULL DEFAULT 0,
  `autorID` int(11) NOT NULL,
  `unix` int(11) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_platba_cas_pob
CREATE TABLE IF NOT EXISTS `spin_platba_cas_pob` (
  `platbaID` int(11) NOT NULL,
  `pokladnaID` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_platba_kredit
CREATE TABLE IF NOT EXISTS `spin_platba_kredit` (
  `ID` int(5) NOT NULL AUTO_INCREMENT,
  `pokladnaID` int(2) NOT NULL,
  `kategorie` int(1) NOT NULL,
  `platba` int(8) NOT NULL,
  `kredity` int(8) NOT NULL,
  `popis` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `platnost` int(1) DEFAULT 0,
  `visible` int(1) NOT NULL DEFAULT 0,
  `autorID` int(11) NOT NULL,
  `unix` int(11) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM AUTO_INCREMENT=64 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_platba_kredit_cas
CREATE TABLE IF NOT EXISTS `spin_platba_kredit_cas` (
  `ID` int(5) NOT NULL AUTO_INCREMENT,
  `verze` int(11) NOT NULL,
  `pokladnaID` int(2) NOT NULL,
  `kategorie` int(1) NOT NULL,
  `platba` int(8) NOT NULL,
  `kredity` int(8) NOT NULL,
  `pocetLidi` int(11) NOT NULL,
  `popis` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `platnost` int(1) DEFAULT 0,
  `visible` int(1) NOT NULL DEFAULT 0,
  `autorID` int(11) NOT NULL,
  `unix` int(11) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_pobocka
CREATE TABLE IF NOT EXISTS `spin_pobocka` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Nazev` char(50) COLLATE utf8_czech_ci NOT NULL,
  `Popis` char(200) COLLATE utf8_czech_ci NOT NULL,
  `pocetM` int(11) NOT NULL DEFAULT 0,
  `kredit` int(11) NOT NULL DEFAULT 10,
  `minuty` int(3) DEFAULT NULL,
  `visible` tinyint(4) NOT NULL DEFAULT 1,
  `color1` varchar(20) COLLATE utf8_czech_ci NOT NULL,
  `color2` varchar(20) COLLATE utf8_czech_ci NOT NULL,
  `poradi` tinyint(3) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_pokladna
CREATE TABLE IF NOT EXISTS `spin_pokladna` (
  `ID` int(11) NOT NULL,
  `Nazev` varchar(50) COLLATE utf8_czech_ci NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_user_pausal
CREATE TABLE IF NOT EXISTS `spin_user_pausal` (
  `userID` int(11) NOT NULL,
  `pokladnaID` int(11) NOT NULL,
  `platnostOdUnix` int(11) NOT NULL,
  `platnostDoUnix` int(11) NOT NULL,
  `unixTime` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_user_transakce
CREATE TABLE IF NOT EXISTS `spin_user_transakce` (
  `userID` int(11) NOT NULL,
  `pokladnaID` int(2) NOT NULL,
  `provedlID` int(11) NOT NULL,
  `status` char(1) CHARACTER SET utf8 NOT NULL,
  `typ` char(2) CHARACTER SET utf8 NOT NULL DEFAULT 'k',
  `kredit` int(5) NOT NULL,
  `castka` int(5) DEFAULT NULL,
  `doPokladny` tinyint(3) unsigned NOT NULL DEFAULT 1,
  `spinID` int(6) DEFAULT 0,
  `doklad` int(6) unsigned DEFAULT 0,
  `zbytek` int(5) NOT NULL DEFAULT 0,
  `zbyvaDni` int(11) NOT NULL,
  `increment` int(2) DEFAULT 0,
  `timestamp` timestamp NOT NULL DEFAULT current_timestamp(),
  `unixTime` int(11) NOT NULL,
  `platnostOd` date DEFAULT NULL,
  `platnostDo` date DEFAULT NULL,
  `platnostOdUnix` int(11) DEFAULT NULL,
  `platnostDoUnix` int(11) DEFAULT NULL,
  KEY `user_Id` (`userID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci ROW_FORMAT=DYNAMIC;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_user_zbytek_kredit
CREATE TABLE IF NOT EXISTS `spin_user_zbytek_kredit` (
  `userID` int(8) NOT NULL,
  `pokladnaID` int(8) NOT NULL,
  `kredit` int(8) NOT NULL DEFAULT 0,
  `kreditUnixTime` int(11) NOT NULL DEFAULT 0,
  `rezervace` int(8) NOT NULL DEFAULT 0,
  `rezervaceUnixTime` int(11) NOT NULL DEFAULT 0,
  KEY `userID` (`userID`,`pokladnaID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.spin_user_zbytek_kredit_casovy
CREATE TABLE IF NOT EXISTS `spin_user_zbytek_kredit_casovy` (
  `ID` int(11) NOT NULL,
  `platbaID` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `pokladnaID` int(11) NOT NULL,
  `kredit` int(11) NOT NULL,
  `pocetLidi` int(11) NOT NULL,
  `aktivni` int(1) NOT NULL,
  `pocetMesicu` int(11) NOT NULL,
  `platnostOdUnix` int(11) NOT NULL,
  `platnostDoUnix` int(11) NOT NULL,
  `kreditUnixTime` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.users_8
CREATE TABLE IF NOT EXISTS `users_8` (
  `USER_ID` int(3) unsigned NOT NULL AUTO_INCREMENT,
  `USER_PARENT` int(11) DEFAULT 0,
  `USER_NAZEV` char(50) COLLATE utf8_czech_ci DEFAULT NULL,
  `USER_TITUL` char(30) COLLATE utf8_czech_ci DEFAULT NULL,
  `USER_JMENO` char(50) COLLATE utf8_czech_ci DEFAULT NULL,
  `USER_PRIJMENI` char(80) COLLATE utf8_czech_ci DEFAULT NULL,
  `USER_NAZEV_FIRMY` char(100) CHARACTER SET latin2 COLLATE latin2_czech_cs DEFAULT NULL,
  `USER_NICK` char(25) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `USER_HESLO` char(40) CHARACTER SET cp1250 COLLATE cp1250_czech_cs DEFAULT NULL,
  `USER_TEL` char(20) COLLATE utf8_czech_ci DEFAULT NULL,
  `USER_EMAIL` char(100) COLLATE utf8_czech_ci DEFAULT NULL,
  `USER_ULICE` varchar(50) CHARACTER SET cp1250 COLLATE cp1250_czech_cs DEFAULT NULL,
  `USER_PSC` varchar(20) CHARACTER SET cp1250 COLLATE cp1250_czech_cs DEFAULT NULL,
  `USER_POSTA` varchar(50) CHARACTER SET cp1250 COLLATE cp1250_bin DEFAULT NULL,
  `USER_PLATNOST` tinyint(4) NOT NULL DEFAULT 0,
  `USER_LAST_LOGIN` int(11) DEFAULT NULL,
  `USER_DELETED` tinyint(4) NOT NULL DEFAULT 0,
  `USER_NUMBER` smallint(6) NOT NULL DEFAULT 0,
  `USER_POPIS` varchar(255) COLLATE utf8_czech_ci DEFAULT NULL,
  `gdprBase` tinyint(3) unsigned DEFAULT NULL,
  `gdprDate` datetime DEFAULT NULL,
  `gdprNews` tinyint(3) unsigned DEFAULT NULL,
  `gdprNewsDate` datetime DEFAULT NULL,
  PRIMARY KEY (`USER_ID`),
  KEY `i_nick` (`USER_NICK`),
  KEY `USER_DELETED` (`USER_DELETED`),
  KEY `USER_NAZEV` (`USER_NAZEV`(1))
) ENGINE=MyISAM AUTO_INCREMENT=2232 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.users_category
CREATE TABLE IF NOT EXISTS `users_category` (
  `id` int(10) unsigned NOT NULL,
  `pobocka_id` int(10) unsigned NOT NULL,
  `nazev` varchar(30) NOT NULL,
  `popis` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `pobocka_id` (`pobocka_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='kategorie zakazniku';

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.users_category_binding
CREATE TABLE IF NOT EXISTS `users_category_binding` (
  `user_id` int(10) unsigned NOT NULL,
  `category_id` int(10) unsigned NOT NULL,
  KEY `user_id` (`user_id`) USING BTREE,
  KEY `category` (`category_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.users_gdpr_log
CREATE TABLE IF NOT EXISTS `users_gdpr_log` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(10) unsigned NOT NULL,
  `type` varchar(10) NOT NULL,
  `value` tinyint(3) unsigned NOT NULL,
  `ts` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `userId` (`userId`)
) ENGINE=MyISAM AUTO_INCREMENT=1911 DEFAULT CHARSET=utf8;

-- Export dat nebyl vybrán.

-- Exportování struktury pro tabulka diva2_akt.uvody
CREATE TABLE IF NOT EXISTS `uvody` (
  `UVOD_ID` int(6) NOT NULL AUTO_INCREMENT,
  `UVOD_CAT_ID` int(6) NOT NULL DEFAULT 0,
  `UVOD_ROK` int(4) NOT NULL DEFAULT 0,
  `UVOD_NADPIS` char(255) COLLATE utf8_czech_ci NOT NULL,
  `UVOD_OBSAH` mediumtext COLLATE utf8_czech_ci NOT NULL,
  `UVOD_DATUM` date NOT NULL DEFAULT '0000-00-00',
  `UVOD_AUTOR` int(6) NOT NULL DEFAULT 0,
  PRIMARY KEY (`UVOD_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

-- Export dat nebyl vybrán.

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

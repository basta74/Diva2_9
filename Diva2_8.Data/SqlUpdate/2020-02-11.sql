
DROP TABLE IF EXISTS `spin_ini_main`;
CREATE TABLE IF NOT EXISTS `spin_ini_main` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Values` text COLLATE utf8_czech_ci NOT NULL,
  `Styles` text COLLATE utf8_czech_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

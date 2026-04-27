ALTER TABLE `spin_user_zbytek_kredit_casovy`	DROP COLUMN `ID`;
ALTER TABLE `spin_user_zbytek_kredit_casovy`
	ADD COLUMN `ID` INT(11) NOT NULL AUTO_INCREMENT FIRST,
	ADD PRIMARY KEY (`ID`);


ALTER TABLE `spin_user_zbytek_kredit_casovy`
	ADD COLUMN `prodlouzeno` BIT(1) NOT NULL DEFAULT b'0' AFTER `platny`;



CREATE TABLE IF NOT EXISTS `spin_user_zbytek_kredit_casovy_log` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) unsigned NOT NULL DEFAULT 0,
  `PlatbaId` int(11) unsigned NOT NULL DEFAULT 0,
  `UnixFrom` int(11) unsigned NOT NULL DEFAULT 0 COMMENT 'puvodni datum',
  `Days` smallint(5) unsigned NOT NULL DEFAULT 0,
  `UnixTo` int(11) unsigned NOT NULL DEFAULT 0 COMMENT 'nove datum',
  `Ts` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`ID`),
  KEY `UserId` (`UserId`),
  KEY `PlatbaId` (`PlatbaId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;

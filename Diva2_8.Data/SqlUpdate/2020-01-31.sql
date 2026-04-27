ALTER TABLE `spin_user_transakce`
	ADD COLUMN `id` INT(11) NOT NULL AUTO_INCREMENT FIRST,
	ADD COLUMN `datum` DATETIME NOT NULL DEFAULT CURDATE() AFTER `timestamp`,
	ADD PRIMARY KEY (`id`);

ALTER TABLE `spin_user_transakce`
	CHANGE COLUMN `platnostOd` `platnostOd` DATETIME NULL DEFAULT NULL AFTER `unixTime`,
	CHANGE COLUMN `platnostDo` `platnostDo` DATETIME NULL DEFAULT NULL AFTER `platnostOd`;

UPDATE 
 spin_user_transakce t
 
  SET t.datum = CAST( DATE_FORMAT(t.timestamp , '%Y-%m-%d %H:%i:%s' ) AS DATETIME ) 
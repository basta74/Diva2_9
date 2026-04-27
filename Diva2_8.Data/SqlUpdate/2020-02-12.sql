ALTER TABLE `spin_user_zbytek_kredit_casovy`
	ADD COLUMN `platny` BIT NOT NULL DEFAULT 1 AFTER `kreditUnixTime`;


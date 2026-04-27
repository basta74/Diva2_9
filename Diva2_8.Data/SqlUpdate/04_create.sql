CREATE TABLE `spin_roleclaim` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RoleId` int NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_spin_roleclaim` PRIMARY KEY (`Id`)
);

CREATE TABLE `spin_userclaim` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_spin_userclaim` PRIMARY KEY (`Id`)
);

CREATE TABLE `spin_userlogin` (
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
    `UserId` int NOT NULL,
    CONSTRAINT `PK_spin_userlogin` PRIMARY KEY (`LoginProvider`, `ProviderKey`)
);

CREATE TABLE `spin_usertoken` (
    `UserId` int NOT NULL,
    `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_spin_usertoken` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`)
);

CREATE UNIQUE INDEX `UserNameIndex` ON `users_8` (`NormalizedUserName`);


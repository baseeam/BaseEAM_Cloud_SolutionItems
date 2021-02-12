DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `InsertLocaleStringResource`(
 IN ResourceName varchar(200),
 In ResourceValue longtext)
BEGIN
 IF NOT EXISTS(SELECT * FROM LocaleStringResource WHERE LocaleStringResource.ResourceName = ResourceName)
   THEN
	INSERT INTO `baseeam`.`localestringresource` (`LanguageId`, `ResourceName`, `ResourceValue`, `IsNew`, `IsDeleted`, `CreatedUser`, `CreatedDateTime`, `ModifiedUser`, `ModifiedDateTime`) 
	VALUES ('1', ResourceName, ResourceValue, '0', '0', '', NOW(), '', NOW());

 END IF;
END$$
DELIMITER ;
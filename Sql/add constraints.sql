﻿ALTER TABLE [dbo].[Scenarios] ADD [Access] [int] NOT NULL DEFAULT 0;
GO
ALTER TABLE [dbo].[ActivityTeachingResources] ALTER COLUMN [Description] [nvarchar](128) NOT NULL;
GO
ALTER TABLE [dbo].[ActivityTeachingResources] ALTER COLUMN [ResourceFilename] [nvarchar](256) NULL;
GO
ALTER TABLE [dbo].[HotDrinks] ALTER COLUMN [Description] [nvarchar](64) NOT NULL;
GO
ALTER TABLE [dbo].[ScenarioResources] ALTER COLUMN [Description] [nvarchar](128) NOT NULL;
GO
ALTER TABLE [dbo].[ScenarioResources] ALTER COLUMN [ResourceFilename] [nvarchar](256) NULL;
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'Unique_RoleDescription')  ALTER TABLE ProfessionalRoles DROP CONSTRAINT [Unique_RoleDescription];
GO
ALTER TABLE dbo.[ProfessionalRoles] ADD CONSTRAINT Unique_ProfessionalRoles_CategoryDescription UNIQUE([Category],[Description]);
GO
ALTER TABLE dbo.[HotDrinks] ADD CONSTRAINT Unique_HotDrinks_Description UNIQUE([Description]);
GO
ALTER TABLE dbo.[Institutions] ADD CONSTRAINT Unique_Institutions_LocaleCodeName UNIQUE([LocaleCode],[Name]);
GO
ALTER TABLE dbo.[Departments] ADD CONSTRAINT Unique_Departments_InstitutionIdName UNIQUE([InstitutionId],[Name]);
GO
ALTER TABLE dbo.[Departments] ADD CONSTRAINT Unique_Departments_InstitutionIdAbbreviation UNIQUE([InstitutionId],[Abbreviation]);
GO
ALTER TABLE dbo.[Scenarios] ADD CONSTRAINT Unique_Scenarios_DepartmentIdBriefDescription UNIQUE([DepartmentId],[BriefDescription]);
GO
ALTER TABLE dbo.[Manequins] ADD CONSTRAINT Unique_Manequins_DepartmentIdDescription UNIQUE([DepartmentId],[Description]);
GO
CREATE UNIQUE NONCLUSTERED INDEX Unique_ScenarioResources_ScenarioIdResourceFilename ON dbo.[ScenarioResources]([ScenarioId],[ResourceFilename]) WHERE ResourceFilename IS NOT NULL;
GO
CREATE FUNCTION OtherTeachingResourcesWithSameFilename
(
	-- Add the parameters for the function here
	@courseActivityId uniqueidentifier,
	@activityTeachingResourceId uniqueidentifier,
	@filename nvarchar
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar bit

	-- Add the T-SQL statements to compute the return value here
	SET @ResultVar = CASE 
		WHEN @filename IS NOT NULL AND EXISTS(
			SELECT 1
			FROM dbo.CourseActivities as allCa
			INNER JOIN dbo.ActivityTeachingResources as atr ON atr.CourseActivityId = allCa.Id
			WHERE allCa.CourseTypeId IN 
				(SELECT ca.CourseTypeId
				 FROM dbo.CourseActivities as ca
				 WHERE ca.Id = @CourseActivityId)
			AND atr.ResourceFilename = @filename AND atr.Id <> @activityTeachingResourceId
		)
		THEN 1
		ELSE 0
	END

	-- Return the result of the function
	RETURN @ResultVar

END;
GO
ALTER TABLE [dbo].[ActivityTeachingResources]  WITH CHECK ADD  CONSTRAINT [CheckCousinsFilenames] CHECK  (([dbo].[OtherTeachingResourcesWithSameFilename]([CourseActivityId],[Id],[ResourceFileName])=(0)));
GO
ALTER TABLE [dbo].[ActivityTeachingResources] CHECK CONSTRAINT [CheckCousinsFilenames];
GO
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201609112039029_ScenarioAccess', N'SP.DataAccess.MedSimDbContext',  0x1F8B0800000000000400ED7D5B6F1D3992E6FB02FB1F043DED0E6A2CDBD5DDE831EC19B865BBDB685B362C57ED625E84D4399494A87332D599795C1616FBCBF6617FD2FC8561DE7989208397BC1C8D510F251F924146F06390116446FCC7FFFBFF2FFFEDFB7E77F28D15659A67AF4E9F3D797A7AC2B24DBE4DB3DB57A787EAE69FFF7CFA6FFFFADFFFDBCBB7DBFDF7935FFB7A3FD7F578CBAC7C757A5755F72FCECECACD1DDB27E5937DBA29F232BFA99E6CF2FD59B2CDCF9E3F7DFA2F67CF9E9D314EE294D33A3979F9E59055E99E35FFE0FF3CCFB30DBBAF0EC9EE63BE65BBB2FB9D975C36544F2E923D2BEF930D7B757AF9F9C99BA44A5E6F36AC2C4F4F5EEFD2840FE292ED6E4E4F922CCBABA4E2437CF14BC92EAB22CF6E2FEFF90FC9EEEBC33DE3F56E925DC9BAA1BF18AB53B978FABCE6E26C6CD893DA1CCA2ADF3B127CF673279633B5B997704F07B171C1BDE502AE1E6AAE1BE1BD3A7DBDA9D26FF54F2CD9DCF1D9FDC2CAFC506CB830D4CE5F9CEF8ABAA122EB2718859F4EA47A3F0D30E168AAFFFBE9E4FCB0AB0E057B95B1435524BB9F4E3E1FAE77E9E6EFECE16BFE1BCB5E6587DD4E1C3D1F3F2F937EE03F7D2EF27B56540F5FD84DC7D3FBEDE9C999DCEE4C6D383413DAB4DCFDF590F2BF2F78DFC9F58E0DD810047159E505FB2BCB5891546CFB39A92A56F0A9BDC833A675AC7473CE4553B25E64D64ECDC4DEB07253A4F72D32BAA9A90A3E05A7271F93EF1F58765BDDF1C5FBFCCFA727EFD2EF6CDBFFD211FF254BF98AE68DAAE2C09C3BEFA7F95DBA6319FFC53082E77FFC1369044A8717C9B7F4B691B82AC4BBBC64998A37BEEABFB05D53BFBC4BEFDBC58FA2F30A25F2AEC8F75FF29D616D606DAF2EBBA5F335F724F035296E59E52009094EA000E42A57FD1F7C04A9C2AEB9E630B69E394BF55E16222B2FCF46FD63D44AB07CE83A096EBF9C466A65A5EB2543E5CB5D5E111B6030F3D183E34867D3863DA733746892D414DDA30B17DF7697D661EA32F756826E3ACCA0BB286C9A6B22BA2B320B358E0D6CD4C574564CB511768C4DC2D471374564F5DBD4FF710004BAB9AC92A2FAA5DAF49D71F9B0AFE9DEFDFCF5E6503483F99866654FEC7D56FDFCDC99D2BB64C3E7E1E1236315870C3836DB39F49E33B567995D8D9B097DE230E8908BD3B41C4BF37C1F380A591E104197F6FD3C29736426F1769FA4BB4BF68F0337BA8793F55F1E2A77989C7381728CC640DC87A4ACB8219EDEA471E87572BAC8BF7046D3826D8318FDCCE1926ED2FB24AB7E4DB72C9FCE2E51D9606C7B9D6C7EBB3CECF749F1305BBFE709C7C66E27882DE79B4192B943A4D1D6EFF2629F382FE028365AB46D5EB5BF88A702B76DFE4DF2606643A8A58F7C2844073BD6F01B5F3B918611B615BA8E8041CAE5C86143A9E4776212D62C41A4726D44B46225B388A59A7EA2BEDCF0A55EA479A7C9EA4E087CC0AD107EA0CA66BEC0169EFCF113E5C724ABD57346614CAE8E7124D6B2B02255F5E7E173C1B802E087342213627D031763353B1B425D7F3EFAA925B22154377031D4B233315675E5613CCE81431F8B21B5A4976A4A09A8E2AA92F4631F38D4BAE04AAE2B0D162AD7860B56721D30785AB689B76F6416B352CB246EB5AA2B17664103C3147F87056B1A8AA3CDCBF76157B3973739124763C3DCAA1D84CD08CDB6EDE331CB433C64C43327E607D34FA5016BA63F80BA2D9BB6D50F9F118429D23DDF1F9F4E71CD5783427561F839229A59AE09843A89AE4B8EDD8A859ABD7FCBAB375C88BFF19FBFA5DB7033FA234B76DEB42C6BDF74E07334E7E0431E62F3B91F51C923ED2A1B47DBD4A18CB8ADE837EA7AA60C83AE8BAFC42E8141EB75104D0B548CA070DB8B0737755BB7F9A16CA1659C66878AC5D1799F8A2D2BC29CF3C209C86B08EFCBE6022F585B5EA67B8E056E8BE787F2B23EF1EFCBA071614F60167055BA3C9F30EA2EA896EDD904A4E8A6F3B146BC8384B532E9DA724A7F267D5B21F8353DA7C5C737D80ACEC13F6869609C9FA5FC8462EF665F215693C0D69C3E43A55F93DF10AD4A636936FFA1D2AFC18788D5A43164F6253A9E804615EE760AEADBFD380901DD5C986F31277A57EA6F275A37F8E1CDE394CF2461F05B5F55C6B5B402CE2B96F1CF696F759DA690FCC16A76AB4BA81BC1F06A5971533975C90F750374B3A497EBF5F575C1BEA589A5F3DA7E8BDFF9DB7D3BFBE75C64B77931D87BCDA2E80BAD765E56F2CE375CFCB083DEEB45796A5496412B155633E655ED633710197071ED5887AEFA80DCF5E378BF461CBFD6C8C08752D7CE8FDAC09FAFFED067B38694CE956616DEA4DA34EEE426AEFCA96BCFC6985E1FE448AD666245ABEBCA03C534687A32180472B969B8510FFF323C7DF6E3B1F5D237B7FDB997727B2BBD90F5BBC6251DB3636DEFE447C2B13FB6703D77862853EB0914D3BE53BDA4313063AB6B7F65436186BC9A7DD6F01A56EE8ACFD41613FE4F7F787487E97A234CAB43DD77E03DF3FB8C1F3F1B263EB05AE4F33DECE67FA737E986CFF66C7D7E2ED2FAF1FA79BEE32BD88418A5AF9676D8945DB24D9E6D67EB3DF44DB2A604B177C9C68ADAE9C85CDBEFC84D64C0F6D8D23AD4294D9DA81B9A9593505347503F203742F915C2045245DB8CB17AAE070AF3C58D2021F0BA062A3789D9FF6A467BB93AC54357C3C8B137B1D4F1BB2816824AA12A932035523FA7B50EB6AB048EB229330DAFAD10D75A15C883D62A546E1A63546B55980F57535568BAB49D4AB4518511AF3E188132D67599C4B6D35B7EC351C08793EC6AF4869E7CCBF33CBB498B3DF090D1D6B27B4D10FADCE87DF9A9B84DB2B41CDF4FC57A4B39C58766C677D5A0A69ECADCF73B8C5A0DFD201E2495073021945BB830D7D4D8B05477E643596630334A251B47F6EA3A5B8436419E18AF3D6A15BBD324BE18C2B7FA110C74572FCBAEDE68F89E13AB7FCB57EE7C74166FD2448CBE613709474BB4CDB2A31779CFEC3E256037ACA86336B83D6B7D93B2EA22AF5839F9343660C17777E7980B65F97B5E6CFF96947786A1F33F230CFD926D0E05D7539755B2BF9FBCB7CF77FC007771D85F3393432A7A5FD1A6E6EBEFF5C34E7E367D9BD5AD82E97DC837BFE587EA6DB6AD3F8DF30812331088329C766379C7C1CCB67CF7ABF7A99057F6BFF073E64CDACDFA4A6D38415B4E4A5A7DF898A454D3EC5C535D67AFDE2E49117F817414EBAA21E7BAA6D438CEAE8A9FD3D1EA8789761C35B210CBC74B7E67AEF76E7B684E6B416033F253F3EE2D74CB209557BD9D9151B53A854BADCD7451125C1D8406032FCC3C958F3DE0B0FB0F2DD1418315B421C3B55C07FC21BFC55CDDE28CF6D5608CB4A5464874559C9DC45E2626D9B874332B83044D5344469D63572FB82621DBBAAFCBFB0B56D50780665771887B2D373C569BD73FAEC2FB2D6B8449395C055A578D84DB073C139FC59B9E7E4D7687D85DB97ED3AB6F99CE9FF8EA248EE4DA60DC4F63DC33802D3AA1880F3F577F43218BE5515E8920D3B2C073C139E2B819AF0D8C676DF743F39CDF9E9AE213C7E30DC00AC824508FC8AD5B4B8D6DC7E6D35FAB4432F8ECD72C64FEC8BB2238D9D4FD10687CACC7B547FB09964FAC8C58AE91D934846ACB782A98293EBCC18762FC02C7A19903F7B46F723CBE13F1D31F70FBA50FD30EDF8A443DEDCEF8D1C87A8E836E5F8F847CB046FA82045E58F39D9CA2E904FA9989C6B647E8AC21B4864F0CADA1F1D2DA6011D3FA87813CB781DCC6DE922FA66901B8C2ED5DAFC8D8661B178E84339975EB1DA3C76ED18671E261CB855E6811AC38F412CC4BE3F617387435DBB77864C65A0C232CD2A77BFE5F2B78DEEAA93680F9EE2FF046A7B984F3B9D1691A2E87BAA6FBCF6D2CD982B64B7795397952FDFE4EC615DBCAC850784E141C4A6273EECE9DEFB1E8EF6DB55B5FF2A35BA5E523D394C7EFD60283FA88B3365658DE61667F1A15FF71BB76BDEFF0203EDA6616E945859597302ED417CBE307A054AE8C144C5C1A1A12B836B50EDAEA0D94FD75A8406439750A3D4F27ECEB4AF8035785ECFC283E8E827609DA10DBBDE7F2C53675FD50DB18BFE926ADB989DF954DA22FECEFCE5C3927EB0B2FFDB00A7DB0E2E395E5E1FC54111BF34365E835CE6B3062289BE793B0F821DF243B76CE0998A43B4540962AA9E3B16CEBCF3AFEBD0100DAFD1F689C5B184DEAF535B2F926E76B86209EEC96D20C3FF3B64B15761DB6655798C683CA75272154C9FF0360F890171051443DB8D9228F4C76509D748F357139DDC9740017F94AA96DB0A4D3695437EE6EA1C954D511EF51CD87697558AB512CA176BA752979292E2D52A749BB05FA64DDFC5B72BBE51607EC2F05ABFA3EAE88F04C9E64B279CD5F1FB6893E737D8B1F4766A01B9A4772223F76B42FE93FF231EC4243D770CD7397946C7BC17E0FFE1CBDA3557F92ECF17972B23B3F14F5676C0F3DA1CF45BA1929B14DBA4F76A7279F0BFE5703E1D3677C822EEBAD8FDB0076B16FF2FD3E6D4E1B3E2314DB7F61496976663FA345D774BF6937878BEB4B6D597E4CF5B4EDC85879BA6F2CE9F1EE0C5F57EA5C5107DAB7BC64C537346DC9201ABD36206CB5122E69ADA6AB981BDD601C725303163252459333562FD2C330F74D4F6FFB589F85F5FCFD7813A6ED8A82641EFF8330430E37A3BA469F83F92B4C9FD7603E29E8486FC1C2F5FE74FB2BA646C9C377361BBA6DC4DD7AE81AFE3022806EF83F39D13DD196784A34262CDAA69D8FEDFBAC893EB6DB05477ABC6463F09EF128EC6AF8323EF9D91856887AA66E0EF86FCB8A1FECAB6807FD4E46E77959D149BA9A5ED44D26AED6F13B65A2FA063D8F06299BEEE8E9AA6A9A663F140DD0CDA2DE0A3E3D879B6453CBDCD93DE619F33CC042C10C2ACC927118F02005F398858A5752EFF0F8F1EAB8B565681367014BCC3AAF63A1F58FE50C7463B90BF10C34695D6B3D08E7012FBA100980F7026F1DC89E0ED6BAF62303A7E550749717156D23992631501DC036EAA715963D33E527BDEEF62DF6528BE6CFF74A1F5343174A1C23FEAE2D3FA9703A97AD3D1585C1552BE7AA70FC96F62363159F5E83C8B48A8AE8947258846AA52095D57F4D4B575B7D8B47A6BAE2ECAB7F29527633EBD75BDE2A2E52A2B27C7FBF63DFD34AFEBAE14D7A7393763169CD04A2643E6E212835BDBC4B6A963FB06FB521E81A399971AEE6CCBDE6105561A18DA0F6089A53EEF4A557608351D399EA691ACF5879BAE4F3F40CB786500D3A57F1773B7A7223C3AEE73FD02118032B9B966664F4B5AEC65D47878556491B395E33D2A59FFBA6A8B77D04977EFFD56FF1FAA90C357B96BD9D33A832A352456FE7FCB585CFED9C61F8584DDAED5CB8D29B6E17C4341E79F8CEE7FF5E97BADB017DCB1FF6C0742A6451EF7B3FC1D31D86C3569AE7A9023B69E2E78F8057CF3E2F9E1FE1B7FCB338A5BD77D5FAF5357C801D27E4AAAB34224C2DD380A55570C3D3612FA0693488DF97EF76C96D393049C5564DB01C0CEB48F8E253B065C5EE814F99F88D832CF78FAC3E010D1F3170259AD4B966F8D89B58EBAF4E9F6A532535F99C1CCAB1F23373E5964181F8735DDEAD640DD2FE0CC66808933C4473B159B8E0A7896699D184FA916DD34DFD840196A95AFD2BDBDC8DA46DB3CB8A9B43CD745A3F99E83BB042A2E0FFE7831A5AFC6C69F1A9BAAB4FE15DED3F3CF54085E8DB0AC3C248693104BC4DCA07EA02ACAFC88AE6910C0D2E7F4B8A6DD0FA937D7861B216692D26EDD1E7F229DB91E52E7C82F5BFD22D59FC17F551866FA39BF6D8E8320FAFCB32DFB49F3C4BA6DBD5F95D5EB2EC6B9D039D733D7A7DE461BCCDB627B2F108371BB7CAD16FDB9BB61FF90CA4F75CE67C53845825F731D852421F6055B5CF7FD2FAFCD2E638AAD2A4CE9FC8659BA459A51FA5D2AC0EEFB273128142857824ABE76FE84F2DE16063599D9FC5494E9481183C4C6743AFCAD9D126BB976702E868586CFEF72679B0C36FA8191771235908647DE9C4B8D2789B114A9A008E063DEFF2629F54C31DB679A6E5CA3886DA7AEE4852E8A3609A14493093B3810996011D4F6DFB8551D527CF1CB26872659BDAF747AC198EB4BEA23BD6D0BE00D4F55566DD296DC2980D913649D1B1D913580B3A472F321999421377543E7DF2C401986257A82A6C2F11E68021C0F9FC1004644219C44A80D7DC864B1CA5569D08B6C1A1D75EEAABCA5065F953F686ED58C54E5E3746511D9AB3DC2450B4133E1C9FF1A178C5964654CC1A65361B688D92A12BCDFE31CC3A70DB1E2F1C40DB357045AC4D4B023DA098838FA353214E617809B829123912AC9912BA63602065771FF120A51670DA9B29A9E1433D2B9EB2EA52C85346AEE693274966A2EDC390C45E3C7AAB29892759C4B88C6658BEB820289DA3D198E65EB2AD0691824593A65E6BE60C4FDAAA053A42770C437753200E15C1DCE843454419882DF1F01C8074CC5E88E1C63795E1882330AD181DB8BED95635F3104A9D3D09A0FD443603C0FD444919182993E0E2C0AF3121A7A773079D466356C8EBBDA37857ABCE0E7654528B221D95E0B1C19C98C7997E1EA0EAF3680790B5EA6F37D12C723009D5D76B38A05092D7398009CD4A370974F1BC79905B5AC8253A176231712C03574C5CC7825538AD1D86174B8EBB112163EE43270F8C393B1E0DEF919067647506AC194541E9BF69CDB1D9F0BE59C776DEA4D3A329BFB6EAFADC58DDB88C6EAC2E0FE3E41A5196D1CC0A5016C4FADD5884546EE8FCBBE4751380AA252172D8991DD2C22DE1DAA24B640E58D28545DA9729B9C816C1AC1F5A67C5E9F23B3789FF2550E9BD93B39B840B6A35B004736E60003127E01034579F40C6E196D8989468242D65419BE682D8C0246586F15C344E883309247C1833200B4BEE8421C09AE989040213C66C39A2C4383042208B495066E17606956691066904A68C9D0BC04CD3AA14A5E69E8C8B08C449CC17F7F49CF8D9607A5DEA2ADB99614F11E0B1AC03FD84E2B3189CA8F89D3FA7B1EAFDB2B82EB936BC44BDC85137D22A59CD9957F47CD0EF9FFCAE9A26F761A117526ADABAC93D584B5E3839DF2D2DEABF1242983978AE8CAD2060E28758D329D9DCCD827E2A12FF33E08E24209A2F408C79B80E243AC06F4ACCCDFF3121CEDE12903A7A1CD5D346B1FA6D0D5D3136C97E6B1DA4F17DD3E4BE04AA0C97003222A92381B531651586165A7EC8112963FA1ABAC2A4E5C8825E7960BD45C22285F9197048910F651878AAC0B911A8A52FB24203CF9819097B78B224BD83218BDAB4A8C3789E137298588E0A6F6AFE201B18D064423A14BA1C57EE804353AADAF11C196708B733A20C910509637DEAEB650166C89763050225790E003C29379307FE2809A788788F0D48BB44E604A75D50446528A54F5BDAB2B16B4363F2EC9856F2924AD0C4E4BC268697FA5B815921B0F0E9504759DFDC39785F94266EF8B23CDA3574B3A04306E1785EB021F2A00CA26F9A66B7EB429FABDF39C4E33CB56766F9C76A1641CD0BD7C0076A2BC0A994AC0D03009CB96D9CF536B5217D8F0593BDCDA5F2206666400DC433A5DBBADDB2E850F3D21967154D5267468B65C734A6B69B1538087F7301089101A57BB9E9A2B01234689B9091B0F3C8C917631EF395BC8F66A0C6DFC524C6E6DDBE24CE8FE4586F4CA183CD362DABDC38F363B2103AA268397BA0DB02ACB74858A3303F03EE28F2A10C43CC9AB32802F5D432366818F2CCE8D8A3C753A5740400100779442B2208B1281B33C2159DB3A3C0AAA0ECED5A12AA3CC98E6BD48B136B431393F3EEBE5EDA6F053BB0165991FA0CC5D6709D015CD7F50C852AC319A04C951465282B88B409B2438B11656F3A61A4D7F50582A28B6329947A877D5A174EEB8F048AC3A6CA8BCEF141008DDAC60D99A6C3A7A913232449CEA0502462439A178298F449A350DAAE0181F6432554796D1BFD92E751937CE6C5A6D77974350AB14DE0EC9581CCD414872A9464C2BE711BFB5A4736328A38664326455E749CC2A9EC17C069EE1CA591D47A1AB4E2DDAD2044A3935C6686AD5570478B5CD21B7D73B329B1BA9A97FA34112C024BDFD7FA6BC5E31059D3119063BB89112974B4688850A2149602A52EA6634625D540C29A4D8CC975DC50D244B0142003ECA215E0514A9D49C9EAA7379838A7E9D2C9FC708667431C2E0F3AD65691E3D425D402DA62823CCC6B09AF60E57936C805865558850BD2DF8EF6B2A1BDD1B76EDB794D7673549B7925E874B095DDEC647F3CAECC3E5E856D1CC72E5E0FE6C8F6B0A32D1C84BA75D9C0EBB07F23D9BEEB411ED5E675B3778370B7263B7715366E1CFB7631CCF5D966D53B21E71B405742103E311A2E88751EC7D27785BE829B01E1BEB2A40C0DA3BDDC4A682250D68C5FD5E1160D48572A82481EEACCF408431BD452313631E9CC0157440694AEEB563341EF2D6F533DF036156FC18A3E7C04DB5EA6FB37D7F5EFEC3B9FB8CDA1ACF27D9265799BE0EB05E7E97C57D4F359BE3AAD8A83FE4AADA67BC92ACB0A2B4F4FDAAA24CDAB295AB9136C9BD0BBC094AAAD83667F040976A719128137C9034E831712C9B4BE419C52EFC725116B9DC818A9D65D4B22D4CD5E6A10533FC1448A0DCA5062EDE3323221F149BB89A4F8E6DB42DC4CD28110E066C5862839352D64DB1D44A7E44243494E0F2E5B357F3D0D77909B0F8521E444B37403E474853A0053FA925165A30ED774589A82DD6F5AA382856D21DD27D683E88D690BC9A8E852DA9950D1A583B39054439D83C0D522E53B129502FADBE94B11ED2D5D5948BB90EAF2D88033DEA709224F10264BF51C662138FA3C7552A357910C6B1339C85D491CDD10E9111FE410559148B28BAF8613EC629551C90901BA8C54A5C06B16E2ED67F73AB1F693774BE3D1BFA313183D28E4993591835C33C4D1990E75FA37B6567999D604B81E8463B4CA35EAA538111A4912A0F935401F98C58F31702A9C59356B824CB537DF44AA18AFAA99280BCC4198C2491997DF58C9CEDC5037544A23214830C2B823C9427A4760108752CFC6885C1D17CA60665845A35044A5134F34BD2571D5FFC171991AD71CDAC2C61CD610179C68095965879207A46830A663CB55320EAD32156B931916DFC74496A5481A4563C75C24C909DF248A63C6440757B73108B6C285D719D056B9C16451C1E17312437A83DA2188AEAFEBC060D724AAD07A9AA8C41055EA212E31834F0FF7CE84852466AA8E33686805C9ADF3331824662238EB21431C48EFD0304BADAB45E3ADAD1C2EA38E0EB40568FE98B832013C5116F9E82D883C6A0D23C84DA789AE4999C96029027E24D3833640AA8E147089B81182A40EFBCE0C93E0D825BE2783B29A707200279ECBCC58A24A106584079898664EF02013E06E68EC328AC2A12E11624B1765415D123E8A6815D80786353A8C93CE5F4912B5D6CC49166AEB6842D6081B4FFCA2433D58B6BDC3FCCAB663C2157136C1FA90C40477BE416A3039404EE0040482AEBF223063ACAB459BF9B67238823A3AC673563FFC7099A8596289872D423303B7F6D6901881DB189348099DCC750ED373F1BACAD757B213CB74AEA5DB5D315DC9175A80310ED53318CD4075D0081F2EBF4C1638440C9089CC43B068C444E8D2E5BF2E1DAC2ACE13D2029291892D124D4054123F512565BE84358B8E9E911DE59B948E3D54B8A4DCE9065D1019A86E99ED494AD16F22BC32B6C7549A2B9B17E1EC4132725CEC9948A60B6AA5686F0A82C5A1A53EB5EDD5E606387FC67690C80CDAD081F05C671E6D1434E139492CAE9826BE24B4E5D42589476DE3C01E12C6358AE8B008B6A0EB26EEAE6ECCED0B88949E0B586299940D58605718834190A4FCBFA0CB00A51F2242FDC992417EE64CB6309B682E5B7FC9A1D96B0192235FD164A626583548CC988B15640ECBC60AB0D63F0923880CCBBF4A998310491932859AA4464D300AF34A48310A49537E1147112A21A92879DAC276172318ED7930314D6F84A1C7CE311B000D691ACD02322675C4D8C2D23A86090BCBE238F549C5E1F0EB7EEC9DE0C03B97434A4A6A07C8054F7A278D1C4C7B270CB77B446BE01D4C7437092AC04C6D18EBC68C6E3A03584E3777516029DCA65E28DDF08C2B04C84486E158CE4516B626E4EC6336917A88C198300B10093DC196C41229C596C09E300683A04849B5C023384A3F40847AC62783FC2CE9A14036F1045180E4D0574E44D280F8620A0D4C3D645E82149C199314852D4523B2628A467B0E48703658DB18EE3E2C4DA3BE6C5CD0D9008EC5FA5487D0CA917DEBC39C20E12EF80C47188F967AC42C5973A6128C69345749982CD1EC2434B2618233EA42B01E8927A32EF490D1CCBAD098BD0115143DE703C02629EB0378B4A0089194E741A43EF15752B6B4031619E30D698240DBC79234DEC1DCAFE7F451D99CDE96162E02B039BEFD45BB8CF31B1C87F8208F284D2C189E9D6920245E2C7902C1F0A67F80088E84B029D1E29ADB99266C549EE25CC0306CA90221B6695F5ED23E7F3304E4D618F4FC0273D6EFDE8837F778651B6BC41B7BC4D3442438D74DBD3402B7AD3B64DBF6DAB21D64B9A2ADDA6D9BF6DAA2DDB66717312EB92D3B6EC97EDBB1E356EC28BB05B760B7EDD76BEB75DB761D45B7C076EB1CF513106758E4504924DEB143C52772D40FF4C37A9ED5E2D4425242D3600C5B29338B05AE942E4B6C0F3DD1489351DE2BD6412E6B324378C4A1ECE5D9E5E68EED93EE879767BCCA86DD578764D73E2FE80B3E26F7F7F5A5D8D8B2FBE5E4F23ED9D42BF09F2F4F4FBEEF7759F9EAF4AEAAEE5F9C9D950DE9F2C93EDD147999DF544F36F9FE2CD9E667CF9F3EFD97B367CFCEF62D8DB38D24E197CA68879EAABC486E99525A2FE72D7B971665F526A992EBA476F79D6FF75A353518A42CBD41D07D6FB6788FFA64D6046AF7544FA1FEBBBBA2F9FCA41EDBEB0D6F58E271211592A390DF71BE6B3F7423024658A03A294EEC7293EC92A20FD32984073DCF77877D86870BC55BB79AB41F854A4B2FA5537EC3CA4D91DED76096894A05747ABD64DEA53BAEF2F74C26AA97EA945F9E29B3A122E04C8380B240558C911048D7FC54FC214143DDD1472564460F8C1A1F1CF609AD746A58AA2B9CA22994B248DF25E4F27218EA8E500198C9A18B110A469086D3E8A3CB8ADBB7BF541B99C6F8AB83FE3914CD4EFAB1B14924052495D029CACF4BB44102C52EDAB2BFAA54E52797D029F6AFC6F85870E2682507BD9CE77B956CFF9BAF6C219A700DDF1E7A0C98FA18EBD07B79BB4FD2DD25B78459B651B628A5C84137F229AAD856039CF83B9DDA87A4ACF8D130BD4901925AA1B37C2FF22FB51FA060F0F489C574DA82D3EBD774CB72F80C80D772E082B1ED75B2F9EDF2B0DF27C503DC115AC9614E130E82DD4E9592F0B3EBDE3926E8D3774F3C79DFC23B1A1231D16D53AB0395FBEE6B60DBE98F3B4D6C7549CBEBC1D64DEDD7B04D2E0A1C3CD09A1B76BAD0F4BEF0C19A4F73388A6D52D5AB0FDE08E5125755548B1C5E257D89C331E6BACCEB3C20CAB965F8954EA98F39C27FFEC6B707657C7A299DF24796EC60AA72C9CAD610763FE8B6829A8C0CBEEB076E3CCDEAE17AEC503104F05AA103428B6D1D125B8267FBD37CFBC1FBB23163955532FEEAB0B3A47B7E4E4B32961FCACBDA38D82BDB0258C1DD2CC7CC701FB7C1511E7D8C5156DD96E090C6C47719E204A6598A17DA91FAC2F5FCECBDD12C3AE9ED23C9D0F96E32CDF8CE35DCF8380E2CAFAFAF0BF62D05F4B75CE262AFB3A28EAE71CE2DE9DBBC50B4B05EEAA0929567C09A2481F215A2D5FC0D803B6E852C442108369199EF70EAEFB55B685EE3CC66D01CFACCDCD2BB4D7CBD23C4F181148350E44293EFE1CD483EB0AA6205ECBCC26B39ECBEFCEFF426DD70B50877025670F0F71569ED58E314F9C2549C7C7291C3C1966DF26C8B50D50A57B362490F14DDD4B040CC5B071B694CEF3F13BA57C929454B5EC168C1BBD4A102E52E36DF799EDDA4C55E753A48052EF486B8FD32B5E167175A9F8ADB244B4BD532960A56B3C8B03742D47515B6A2BCD652D896D8DC4501D7534E9BE28EEF1E19D7F00031B5CCE102E6B0DBE91BF6F8ABCB7ABE49386E4DCB1AA8E04CDFB6C80DD51CFA6A5D936D725DFD0D0350EC403B65D5455E31D5FD3FFEEC882B442DA9652EDABE2C7FCF8BEDDF92F24E55F66289D361E050D4EBAF4AF6F7DA51402C7218E55D9EB18BC3FE5AD5795281173D44A2700D7A0F5F7FAF9FBC736DF4364BAEB5BB48BDD4E19239DFFC961FAAB7D9966B49A65F33EBC51EB48131AB652E7EC95A93BFE310655B7E22A933D1CAEE49AD984EBBDE5E749D36FEBA9A7D504BE6E2BF25AA4978DDB7452B8569B6C6BA439542FF9B8385540FB971B5C967DDF167475ABF26BB0344ACFB7D351072FA28C8F1A60948BAEC7DF144A135BD1D13FFD9E314961190B805799DA556590D2CC16435FE80849274BB439144E5383CF32E97A08BFBB463A100A618E4D7F6C3427CCFF62358F2C4EF01DD5F3B8CB9EDBD771F33951FFB4E3701CDF302C8A4924B5603B931F9913FCE7A1A1EE0C29BAE6B1359DCBA41733BB95B370DA920EB06A1809BBFBC7AF7A6AC50AD5FA9C8C951DDB4F93B539CBF52C15436D44270D03379043880155A3E5E602B89752D62746B015F52F8BCA0388233A5313D4B3C3809642320CB486DA1BBAC80EBF085A63ED254874DADD7542EFF9A223FA8EED5F6A7255F647CC8F9AFACFE145D73290FBF3B7DA051BF27D87E4DF7ECDFF38CA9C757B5D4E5FBAD7AC6B5510EBFBA709CDD42A4C69F57B3D886247C01F65B4BC2C766C35ACE03A5F005D75C58D46F5BD4014905AB99ECD74A9C8C18C774CF43998DC0B49708D03EBB3A378B294C15759E7A1A1E3384373D8EE372FC57474DDC1795D8F0A383517828367749C9B617EC77E5AC2795B853AC2F7D618A6D89E3A67D7E28EA670F0F3D91CF45AA7E7B6DAAE732579B7CBF4F9B53AECE835EEA47F90B4B4A1D5C7AF96A34002D649DBB9F35402B50881CA397B5E7465BDEC2EF8FD8BF4A483BE7BAE574A402761E94C2341B10FFE7F58EEDD17D082A777918D530B37D9F350FF8763BC58F0395BB5057DFD5B4BF381C89183F8A67EAB39FF157A737E61BF6969BCF7B7D37928B9CA5779E971A9B42C1D4CB7DE1C589E6E7735D9A0DA1808589B43F8E73A1985310987EA96C7D10905228464082402F041046324BBBCB169A332C3119758EEAF61E7302379B660E2EEFF2A24217A85EEAF65A1D25AC15BABCC92ED8A6752D2B8FB2C7DF1FF1678AA618C4545C6279D108D8C49B4E83CFBF1429BB4161A4972E8FCFF37C7FBF63DF53F56321F1F7357C84DD62017ACAEDF44E9E71AED06F1DF5D239C2183C9AB54E8B3CEEEE3D0858FF1422C7E83DE8B951E989BF3F62EF819ED9327C770908BE6B2731D169282A0A7E8479162E6BC2EFAA82EEA9E67C36B41EBB468E9AAFBCCF199F9E5EF5418E86CF99095FF3EA8DFC4268D79902F007B36A2F7D0A025D94A489142943935A0B74184DD840BB2C07C481BAEF8DB17746EC94183F54B851C8E779B64D6BC09EBC2F2FF829FCD5E94DB22BF170B6C034AAD922F4A5A1258D50AB0C0BB3FB65F8F79034A24BD8206592688458E78568845776C923D40C0E6D95D393FE01295F630F65C5F6CD3A7B72F98FDDF92E6D5CBF7D858F4996DEB0B2FA9AFFC6B257A7CF9F3E7B7E7AF27A9726659BD2A3CB4DF1627328AB7C9F64595E75093F08C92A9EFD5C27AB60DBFD99DADC3DE5454DA52CB792635150F3164829492BFECE346CF498F9C26E4E30C5FCF24C6DF812D009F5705E9D1EB2F41F07C6459CD51159EA03598DB8FAFBD801756746527A12088CB02ADC17EFB32DFBFEEAF4FF34945E9CBCFFDF572AB19F4E9A279D2F4E9E9EFC5FE79149478C7650D9B7A4BEDCE583F9987CFFC0B2DBEAEED5E9B3E77F76A6AD9F340C1D3CFFE39FC40EAA42FD2453DCE38CB8A16C2734D4E06A15AD8CE94DA0015D15DAB13A8E341858818092C5103C9A9650D0884C720E181F4E3668B4EA96153042991436AA909506E4B5380A7D3C46F96E096E938A5569ADA15CD5A714E0BB2596D67BB2231D20F1053E327DC620BD2E7AAA024024120A02369A27236070084D3AD4814DABCB8A1130AA964490B0E04C1D016382088688094BF3012E010A4125B5474B87D37EF0594E625A8FC015AEA5F308A40724F108E3154FD91172DA02068E65EC88DB8D90BEA3257C9D7ACCBF12B73AF818D2937258D48EBBA896456382232A1011FD988E96CDF0FDF7D885377D473C4069318EE26C45B528FFF8D40700A5BAD5786F11D2E56030B45B426107A4212187BFE2D35370F8D392136F50E938C25CCF5E711420D7725D8481B1FB26D75FB709AAD1770C63A60B7FC880892DC28615C969667797910E27C777AE80F3521CC522BBB01D2F7D9C924BEB7DC7E9AB7F38C2A99BF010207F9C6CA0FDF37367DAFA3B296F7B16CA4D1180369DDC64FEBCFA072CC3808B51823DBC820C13C3BB2AAA85B2A6A3DCD2DE37F28C87CDF3DAF7873FFD6155DA4509C311A80D064A4148C553594476EB30209B45DC2E94B4161D711D14EEA75A35B3852F6947458C06B59FC035640C30774C4E22859138D76747BE1D406B450F3514222A8D5C985212B36F80D628E904564ACFD4FC2C5A29D946741F48E00A5F78FFED1253D0B5386D0756F25E44BEC518B260441E3698092368A96BF40275109A43237C9491D73F908B23648C2AB5100F9090D2232E30D5D41EFE2A4B4EE8218FF27FEC93EFFFD375684A3A8F0814A5741E71E94511A19EB7C39F1690A823EC69879AA0C37F6840460E7F17F4988B435D18B635DAB76C7EFEE9E47DF94BB3CC5F9C7CE5D2A897A8936A266FBEC6E418736CC08D949BC7F07F65192BEA170B9F93DA08CBEA5AAC19AFD734042ACC9644D82BC031494784E52D66E97026E77A2165CB6AB186A7A39670E0400B52A4F963B2F3D6F7CE748D962732EDE10FC9648A53B826AD993D8EC2469AF022C4F1D2D8E3162054F6CEF70051B5D49A2E048E781D5AD36DAC7E43FCB1AD3DB66D4DFE3ADEE5810919F670229047B5E758AFC73C4C292013074D6696C41BD09A3725D600EAC3C183ED33A38C2CF27B14898BC8B497B005E9BE6F63B28E47B5D03C0E779617288B1E10356F2F9641813689904B9AB09C8D492DEC3058F90DD8E24F05C868089CFDB5BF21F17AA1D664E288E0F822BF4679EE3E4A319382F4A08100AAB1AD8C28516E3EDF6728193D0C0CFFE1A9AB2CC7F41E2DD99B5D9E38DF290B893DC854E896169424837A7AC2D26250CE3708108266738A8525E5DA88BEAD991255D066014B4F0154F5756DACC3BD1F61C39CCE610187AB3F8ADD68CA681CEB7B843424D408184C4723CCC321E5E208B8789732708C97BC1E17BC68A68D96EA9E6F8F0FAE64F5E41AFE438492699874FDB3E7B1F6484B568A353823F1D0F23F3C9141FA42906B88CA18C8ACDD07694C8F71141BDA67209386695F7B2A6F6CB4D7497A3A0D7F1DDE26D380152331E8559F47C35FBB2AC9330274BE943403A1B3FAB5E7BC5E80AC1547B15AA63CFEA9092FC226512035E9442658B689A3984FAB0D1C2FE89D9E0BE22824A4278E3048CBE3932B2D3E3FF93A8CF618784C2511F735F0D2C61A19777074F7A3C09E9E1422E4AE340C7B3E5FD4097922405F18EDD579B40F9EFB9C10FEF7557A1688D8125BDBB3A0A359E6B6740E339BDD3FECE820CB55C85210309E91CCDAED68739A88A3D8ACD635678F306A3496FEE128D0011B1A363CD4CCD67F4DFBDD8D10635FD28857709C6E25FF479D4CA119E8106DB863B30E68FFA4FFE9E36157A5F7BB74C3FBE7A8D3C425108183834B44912A7227FFA475F2A5FDC2B04A93FA3BBEB22A92544FD4F1B948B3FA09E44E664AA946C4582DF481A05AC28F042CAB2165E69CD2B361DF3E1BBA51D06F9386947C8186973E4EE6D41019E271EA749A5F8F1D0870BCD135CF7D1B26AD43806DFABBC899FAE4F505AE5020E0292A08A0D09F5342C1010763E8BB05D1D087B41B82FA71D5A6A732D7A6718884A74FE75844C7069A9E44248F579A103F70C8BF6910444BD262C49418E1710DA81AEDAC0910F5F4C913BBC26982A2EA34DB9F1F0970B0F0AF486F2B01493D2772029ED4AA77EA36C06CB63FABFA4665ED53F686D541814F5E6FDA544DE749B949A0677EBCDBE8688D8AAE86E1D990E58465CB878A8BE0ABDD6AA382CBA6771C4F4E478C0E8723D60AB061CAA688CDA8185A4A9C50E977B7BD6965A6331A3C6B22E0F8DBCFF66C923383A809A881AB160FEC4CB473A9315AA4B3B55AF61850660C4A8374893E3B9F1D566AD8CAC810B3ED5F1851A0F43160C5122514E9D4163B660ED000C10B7AB34B0FBF8383080A1122CE3B58EE0A2A282010609B41B5260199352ECA9460B34647423A2705F6581C7CF5AC8A3F2F8C3C7538C839DFD4C963C09CC6E3B1014EDF229DB4DC245BE5DAB4DA225BA7AF365BC3160A806A0CD193B48F12E74594142108F65E0AE58F08457868A415E3A78F6F73453AAE0FD170C4791D7F74F22378A02F123CE0983ED360C3158A40CCE3C5F54A13F9E6B8DC046DFC21D84DD0953D06CD630CB88474B9AC9B40898FE2E42B5023054960D30A8FCE6B600C84340D7E3C5D0794483D8BA069591C2DB7A32D811DE79D0DCF3CB0CC4D5B1B66458C8C64B865EB62B2488AA1FF8D8E10314892484AFA7D9ADB3528A80C32537814192780A011A1BCBA9D0111C280852F29705078CCA6091C421236919AF8F324D0709DA740BD81E59AC37A33C5775B00189AFEA2A80F2FA44C7244360CDFB80F4EAFA16686A189C163C1A57EFEF10167E079E8BF184C17396A05627535672ED1AB3081B77B62A782B638D4A2C7E55220A37B518FC2789C70F125389FB5D6EF3F703D572DE1393025785E0A2FB38264BE6F489680C3D162A09E148AC5E7818849F62468D4C8B380C9CDC72580A6307624B0EBC3190917B5FD4F38E486F886E2F48E3FFA5CCF8224A1E249F002076C9C122D789848A4573C72E3DC485142FFCD001335D820446F283B7A8018432BAE1D1D4D7CBB2BBA0A69E3E14113DA95B8A38404B7C8B800C2FA4D0B0E1A2AFA28C6CB424288947725098C000F31CA1E8812A9823B581CF1171B366810C169D14387AB1A9B71E9B3B15DB344B59096D029F31E5E9D14CA0A0EAC02163E1DEAF84C9BBBE83633E94BE81F66F3693F016976BB2E644CE56F9BC4705EEE89C6BCC0F27C9CB10264D5116CAD9EB926CCAD387FED0FEBF5C6E97179A799770785520F69D959EEDEDB7F64ACE25A2D70B657B68FAC70C265712F3AFDC2F6518F63A60326094747A9EEC9685B8182EFBFB611BCA0FD4F380E8670B3E2F48D3FFA7841419250F124F880E3E74E830E5BD45EA45731D6E8A248E9832C5CC1C3F787C924474C2DCA2C3496896362CC892E7354DDB5624BD881ECDA27EA2EE484D1A3DC8D9CD0B7821D498BB944BD0E5E5760AF755C072F11C6C9E33A7805E19C40D8D1E20E4C10F46B3D51069642907370817561A87E145A1C36555E403661087848A67523414E8748F3D8B152734AEA5199953520C57EE059CBC6B6C459695E48399D9556A3706A5BD23370BF4350DB638A42E818C776E920FE681ADBB931E41E07272E8C5619056766300505C159259E482F2BA700D2E2AF2B17818EEBFBCAB5626608DF333B68968E98B4146CDC0226AD1537D4037574D82C7B87B114683CCECC2BC08C94BC869256227A029BA5F249CC9DC8868C487178EB486B457A4C457ECBB0FE8F161D9E2F2CF2C1E22A9C36FE5652148CACD33A3A16CB6825D871B088E280662596D0EAADA0F5E0836CFD4403C83AAC9EF55B3CEBC108D5D2890691355838ABB76E16C30796F6CFF96E206A16C563BA37084A9CB8DC2D0236ECE59038E4EDBEAAC3D118903626F8D643F940A7D6471A56084B743E0DD43CA20AD5156782D3DB2663396F53F116AC1874EB96BD4B8BB27A9354C97502BCA8A85B5DB2CAB2264E4FDE0E19D1AD9AEC7273C7F6C9ABD3ED75BDE5B559D6B1DA25A0E7E42161AA4C1B1056111A0EA6DBAD83E98E017AE75D01D859534625DEE4E546E8376578174D42715A2FBDFF0DE9A82FC6FBEA733AD2BA6BFD7348676D21DE55EB68A47534A6FF443A1B2BE01D0AF95069BDB6CF49901EDB42BCB7BADCA523F1F9A3A14BB19AB973F199A86D18C6CE6D5DBA740478EF1066A53A38A7925BD2D6BBB95F6B8FE35661EF4ACBFDA76B5AB586B9CB2E23227161422E2F6C9D42750DCB16F2F5D94605669BD2C603D68246025474596A96616015CDCBCD67308A27C1A048855A668D2AB84A6C6318B3A3681D8F45506F7DA9CB2AE8525B18564157C3BC0ABA841FD685AEC586D557BB5605EA59ADE5DEB514F7D53A0AA9366540521C5DDBD8CC63B1F6EDD4D710835E47755F0222B92D74811632C16A0533B068933B3A86B5EEC622A8A3D1C34DD70C86CEA04A66BD401F80164A0B6575A861E278880F46EDB78BB883F6DA959BFAECC20A917B94A207E11D4BD58CFD0B35EDC368BFF5D5BA6D7F86BA693F4CB6911D1D901AE9B108223F7A52E958357406553263953E00FDA3459457B3E5A8D6A2EA1FA3EEB1E91D58E7089E0055D2A8F3F2446824899DE6EE04DDE403C3E34F9AC3C3EEBD14A9205554D7942C0007E108D6322E8FB1527411A886BED0B0F93512A3D2731D03AF4A3DDBC065DF8130F6BEC0CA3A4160DE4CF706FE55FF0747536A843EDAC2C686EA6C10D8198B0CC2B079DB1A7A78A5D80293BC2D5661A18FC0261094EE441228B43F471286F029A3E805C2A40157B73122BA8B0446DA9FAD62F010660C710C0E3F822CFABAD30AC2510F790841CCF5D0CBB6732940723055C799017C4D0D2FD2EF2BDD50A5D4BD9DDFC92C97AED674E240FC69AD2E55CBE20A00F0335A84A1B79810279847555839065A1EC2C1D3D3C3BE415D588E1470E60D7EC5867DB0DCBE23E1DE537183826A4D285CC0C9E82259CB27FC33881573BD2A5B84896A94A54C4529B1E5D44B7B49345272B8D324674EFD3E85D820DFBA72BE14CA834505A72B078443C86B2EB1A3BAE91B1EC61F0D82F0106420467AF7BC191250D6EE890E0BD2B5837258E8CAC20540C81C0D49C435E1B42C22E486A3959356B8AA03843939324554B30B699EA50426FE852C336B82607986E5AB98765EFBDF0C4C03D7454D5BE9F760A6B1DCB600DFA434B8A12C48EDF587114D73F1E7A802305FEF9925424FB1195544840B4F70BDC545915BC251928EF11367449DB30EC16A89312DBBFBD4A762F8E257D9DB239D7FCD291C014138E47C0CD3330B6FE140AE428A34A616C1343710D6A47C14D68D911BA30B027E51A858DA517731630A394040F49473127BEAA38886A5F147925908D2808AE309457F0D61908839B55A247120CF3C240243593441A889C10C6230E6100359915E8E488C742504719004192200431A2C9330A8D9B360C1000F5C64F94815086272147598D235C2C59E152AA2729D1A2586B44666D68D49908E6F9B753866CD72C09AC7FC9712CC005CE30968A4C18AEFC99A51B63F2C34AB603E158C3963DE951533A9650D316316C82C1211AC24D17830694C8F01304C4FA7210D5F7D4AD8B030FE483AE18134A0E26842D13341182462491B11491CD8E3488942C4570360E202F342A0E0216C3138892FC6831A826D686D43B1EEFC1FD8CC6F1B820C5BEFB209ADA617D4FC17D602DB5AE074B394CC71D6A7114F13579D44214C14468D02D69B8AF979348A318E352A027AF46B64E3243EC4945B2FFEF4DB16B0D9222F9FB71D81225BFC6587CEBFCD61676931B1A86675DB81AC8A5F5612A583C5AD9A443C733E7601B925A8685A74DA49C4339F4920BE56A6BDBEB7056005580AFF5A638E77F7C45B33BC7204737EE19B328937B7DD296467F213CCF23B92DB6EE4B513798A66811DC871F7F1DB79FCC531FF8EE3B6DB78ED34FEE2986F87710E6E0788282C409EFC2223E6076FAB312DB4B86D90108DB1DD642169DF0B5F8E4F56AC6F81A678F4520781ABA90CD1C586B29767ED47CBDD0FFC9F555E24B7ACBD706C7E7D79F6E5C05BEF59FBAF37AC4C6F47122F39CD8C3531E846A27D9DF7D94DDE07555346D457E98BFBCB535625DBA44A5EF38DB9BEA6E4C59BFAC553767B7AF26BB23BF02A6FF7D76CFB3EFB74A8EE0F156799EDAF77D2077375703653FF2FCFB431BFFC74DFBCF68AC1021F66CA59609FB2BF1CD2DD7618F7BB64A72A188C441DF5EDAF8CFFDECE65C5FFCF6E1F064A17794624D4896F0856F795EDEF779C58F929BB4CBE319FB171F87D60B7C9E681FFFE2DDDD62A1F23629F0859EC2FDFA4C96D91ECCB8EC6D89EFF936378BBFFFEAFFF09DB8DB2924F2E0300 , N'6.1.3-40302')
GO
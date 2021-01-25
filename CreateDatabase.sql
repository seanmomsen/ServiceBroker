DROP DATABASE IF EXISTS MessageDemo
GO

CREATE DATABASE MessageDemo
GO

ALTER DATABASE MessageDemo 
SET ENABLE_BROKER;
GO

USE MessageDemo
GO

----------------------------
--XML Schema
----------------------------
CREATE XML SCHEMA COLLECTION MachineEventSchema AS  
N'<?xml version="1.0" encoding="utf-16"?>
<xs:schema attributeFormDefault="unqualified" elementFormDefault="qualified" xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xs:element name="MachineEvent">
    <xs:complexType>
      <xs:sequence>
        <xs:element type="xs:string" name="MachineName"/>
        <xs:element type="xs:short" name="MachineId"/>
        <xs:element type="xs:string" name="EventText"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>';  

----------------------------
--XML Message
----------------------------
CREATE MESSAGE TYPE  
	[//test.com/MachineEvents/SubmitEvent]  
    VALIDATION = WELL_FORMED_XML;

----------------------------
--Contract
----------------------------
CREATE CONTRACT [//test.com/MachineEvents/SubmitContract]
  AUTHORIZATION [dbo]
  ([//test.com/MachineEvents/SubmitEvent] SENT BY INITIATOR)

----------------------------
--MessageQueue
----------------------------
CREATE QUEUE dbo.MessageQueue;

----------------------------
--Service
----------------------------
CREATE SERVICE [//test.com/MachineEvents/MessageService]
AUTHORIZATION [dbo]
ON QUEUE dbo.MessageQueue ([//test.com/MachineEvents/SubmitContract]);
GO

----------------------------
--Send Message
----------------------------
CREATE PROCEDURE [dbo].[Send_Message]
  @MessageText NVARCHAR(MAX)
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @handle UNIQUEIDENTIFIER;

  BEGIN TRANSACTION
    BEGIN DIALOG @handle
      FROM SERVICE [//test.com/MachineEvents/MessageService]
      TO SERVICE '//test.com/MachineEvents/MessageService'
      ON CONTRACT [//test.com/MachineEvents/SubmitContract]
      WITH ENCRYPTION = OFF;

    SEND ON CONVERSATION @handle MESSAGE TYPE [//test.com/MachineEvents/SubmitEvent] (@MessageText);

  COMMIT TRANSACTION
END
GO

----------------------------
--Receive Message
----------------------------
CREATE PROCEDURE [dbo].[Receive_Message]
  @MessageText NVARCHAR(MAX) OUT
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @conversation_handle UNIQUEIDENTIFIER,
          @message_body VARBINARY(MAX),
          @message_type_name VARCHAR(256);

  BEGIN TRANSACTION;
    RECEIVE TOP (1)
        @conversation_handle = [conversation_handle],
        @message_body = [message_body],
        @message_type_name = [message_type_name]
      FROM [dbo].[MessageQueue];

    SET @MessageText = CONVERT(NVARCHAR(MAX), @message_body);
  COMMIT TRANSACTION;
END
GO

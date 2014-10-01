﻿Imports System
Imports System.Xml
Imports System.Security.Cryptography
Imports System.Security.Cryptography.Xml
Imports System.Security.Cryptography.X509Certificates



Module poneX509

    Sub ponerCertificado(ByVal archivo As String)
        Try
            ' Create an XmlDocument object. 
            Dim xmlDoc As New XmlDocument()
            ' Load an XML file into the XmlDocument object.
            xmlDoc.PreserveWhitespace = True
            xmlDoc.Load(archivo)

            ' Open the X.509 "Current User" store in read only mode. 
            Dim store As New X509Store(StoreLocation.CurrentUser)
            store.Open(OpenFlags.ReadOnly)
            ' Place all certificates in an X509Certificate2Collection object. 
            Dim certCollection As X509Certificate2Collection = store.Certificates
            Dim cert As X509Certificate2 = Nothing

            ' Loop through each certificate and find the certificate  
            ' with the appropriate name. 
            Dim c As X509Certificate2
            For Each c In certCollection
                If c.Subject = "juancarrillo" Then
                    cert = c

                    Exit For
                End If
            Next c
            If cert Is Nothing Then
                Throw New CryptographicException("The X.509 certificate could not be found.")
            End If

            ' Close the store.
            store.Close()
            ' Encrypt the "creditcard" element.
            Encrypt(xmlDoc, "creditcard", cert)

            ' Save the XML document.
            xmlDoc.Save(archivo)
            ' Display the encrypted XML to the console.
            Console.WriteLine("Encrypted XML:")
            Console.WriteLine()
            Console.WriteLine(xmlDoc.OuterXml)

        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try

    End Sub 'Main


    Sub Encrypt(ByVal Doc As XmlDocument, ByVal ElementToEncryptName As String, ByVal Cert As X509Certificate2)
        ' Check the arguments.   
        If Doc Is Nothing Then
            Throw New ArgumentNullException("Doc")
        End If
        If ElementToEncryptName Is Nothing Then
            Throw New ArgumentNullException("ElementToEncrypt")
        End If
        If Cert Is Nothing Then
            Throw New ArgumentNullException("Cert")
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''' 
        ' Find the specified element in the XmlDocument 
        ' object and create a new XmlElemnt object. 
        '''''''''''''''''''''''''''''''''''''''''''''''' 
        Dim elementToEncrypt As XmlElement = Doc.GetElementsByTagName(ElementToEncryptName)(0)

        ' Throw an XmlException if the element was not found. 
        If elementToEncrypt Is Nothing Then
            Throw New XmlException("The specified element was not found")
        End If

        '''''''''''''''''''''''''''''''''''''''''''''''' 
        ' Create a new instance of the EncryptedXml class  
        ' and use it to encrypt the XmlElement with the  
        ' X.509 Certificate. 
        '''''''''''''''''''''''''''''''''''''''''''''''' 
        Dim eXml As New EncryptedXml()

        ' Encrypt the element. 
        Dim edElement As EncryptedData = eXml.Encrypt(elementToEncrypt, Cert)
        '''''''''''''''''''''''''''''''''''''''''''''''' 
        ' Replace the element from the original XmlDocument 
        ' object with the EncryptedData element. 
        ''''''''''''''''''''''''''''''''''''''''''''''''
        EncryptedXml.ReplaceElement(elementToEncrypt, edElement, False)
    End Sub
End Module
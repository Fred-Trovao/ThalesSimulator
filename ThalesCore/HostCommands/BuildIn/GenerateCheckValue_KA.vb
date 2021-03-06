''
'' This program is free software; you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation; either version 2 of the License, or
'' (at your option) any later version.
''
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
''
'' You should have received a copy of the GNU General Public License
'' along with this program; if not, write to the Free Software
'' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'' 

Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Generates a key check value (no double-length ZMK).
    ''' </summary>
    ''' <remarks>
    ''' This class implements the KA Racal command.
    ''' </remarks>
    <ThalesCommandCode("KA", "KB", "", "Generates a key check value (no double-length ZMK)")> _
    Public Class GenerateCheckValue_KA
        Inherits AHostCommand

        Private _key As String = ""
        Private _keyTypeCode As String = ""
        Private _checkValueType As String = ""
        Private _delimiter As String = ""

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the KA message parsing fields.
        ''' </remarks>
        Public Sub New()
            ReadXMLDefinitions()
        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            XML.MessageParser.Parse(msg, XMLMessageFields, kvp, XMLParseResult)
            If XMLParseResult = ErrorCodes.ER_00_NO_ERROR Then
                _key = kvp.ItemCombination("Key Scheme", "Key")
                _keyTypeCode = kvp.Item("Key Type Code")
                _checkValueType = kvp.ItemOptional("Key Check Value Type")
            End If
        End Sub

        ''' <summary>
        ''' Creates the response message.
        ''' </summary>
        ''' <remarks>
        ''' This method creates the response message. The message header and message reply code
        ''' are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As Message.MessageResponse
            Dim mr As New MessageResponse

            Dim LMKKeyPair As LMKPairs.LMKPair, var As Integer

            Core.LMKPairs.LMKTypeCodeToLMKPair(_keyTypeCode, LMKKeyPair, var)
            If LMKKeyPair <> LMKPairs.LMKPair.Pair04_05 AndAlso _
               LMKKeyPair <> LMKPairs.LMKPair.Pair06_07 AndAlso _
               LMKKeyPair <> LMKPairs.LMKPair.Pair14_15 AndAlso _
               LMKKeyPair <> LMKPairs.LMKPair.Pair16_17 Then
                mr.AddElement(ErrorCodes.ER_21_INVALID_INDEX_VALUE)
                Return mr
            End If

            Dim cryptKey As New HexKey(_key)
            Dim clearKey As String = Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKKeyPair, "0")
            If Utility.IsParityOK(clearKey, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim checkValue As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("Check value: " + checkValue)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            If _checkValueType = "1" Then
                mr.AddElement(checkValue.Substring(0, 6))
            Else
                mr.AddElement(checkValue)
            End If

            Return mr

        End Function

    End Class

End Namespace


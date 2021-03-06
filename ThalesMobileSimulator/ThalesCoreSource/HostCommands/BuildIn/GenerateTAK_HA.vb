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
    ''' Generates a TAK.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the HA Racal command.
    ''' </remarks>
    <ThalesCommandCode("HA", "HB", "", "Generates a TAK")> _
    Public Class GenerateTAK_HA
        Inherits AHostCommand

        Private _sourceTmk As String
        Private _del As String
        Private _keySchemeZMK As String
        Private _keySchemeLMK As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the HA message parsing fields.
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
                _sourceTmk = kvp.ItemCombination("TMK Scheme", "TMK")
                _del = kvp.ItemOptional("Delimiter")
                _keySchemeZMK = kvp.ItemOptional("Key Scheme TMK")
                _keySchemeLMK = kvp.ItemOptional("Key Scheme LMK")
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

            Dim ks As KeySchemeTable.KeyScheme, tmkKs As KeySchemeTable.KeyScheme

            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeLMK, ks, mr) = False Then Return mr
                If ValidateKeySchemeCode(_keySchemeZMK, tmkKs, mr) = False Then Return mr
            Else
                ks = KeySchemeTable.KeyScheme.SingleDESKey
                tmkKs = KeySchemeTable.KeyScheme.SingleDESKey
            End If

            Dim clearSource As String

            Dim cryptSource As New HexKey(_sourceTmk)
            clearSource = Utility.DecryptUnderLMK(cryptSource.ToString, cryptSource.Scheme, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearSource, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearKey As String = Utility.CreateRandomKey(tmkKs)

            Dim cryptKeyTMK As String = Utility.EncryptUnderZMK(clearSource, clearKey, tmkKs)
            Dim cryptKeyLMK As String = Utility.EncryptUnderLMK(clearKey, ks, LMKPairs.LMKPair.Pair16_17, "0")

            Log.Logger.MinorInfo("TMK (clear): " + clearSource)
            Log.Logger.MinorInfo("TAK (clear): " + clearKey)
            Log.Logger.MinorInfo("TAK (TMK): " + cryptKeyTMK)
            Log.Logger.MinorInfo("TAK (LMK): " + cryptKeyLMK)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)

            mr.AddElement(cryptKeyTMK)
            mr.AddElement(cryptKeyLMK)

            Return mr

        End Function

    End Class

End Namespace

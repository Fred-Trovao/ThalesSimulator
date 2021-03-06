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
    ''' Generates a random key TMK, TPK or PVK and prints it in the clear.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the OE Racal command.
    ''' </remarks>
    <ThalesCommandCode("OE", "OF", "OZ", "Generates a random key TMK, TPK or PVK and prints it in the clear")> _
    Public Class GenerateAndPrintTMPTPKPVK_OE
        Inherits AHostCommand

        Private _keyScheme As String = "Z"
        Private _result As String = ""

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the OE message parsing fields.
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
                _keyScheme = kvp.Item("Key Scheme LMK")
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

            Dim ks As KeySchemeTable.KeyScheme

            If Not IsInAuthorizedState() Then
                Log.Logger.MajorInfo("Can't print clear key while not in the AUTHORIZED state")
                mr.AddElement(ErrorCodes.ER_17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                Return mr
            End If

            If ValidateKeySchemeCode(_keyScheme, ks, mr) = False Then Return mr

            Dim rndKey As String = Utility.CreateRandomKey(ks)
            Dim cryptRndKey As String = Utility.EncryptUnderLMK(rndKey, ks, LMKPairs.LMKPair.Pair14_15, "0")

            Log.Logger.MinorInfo("Key (clear): " + rndKey)
            Log.Logger.MinorInfo("Key (LMK): " + cryptRndKey)

            AddPrinterData("CLEAR KEY")
            AddPrinterData(rndKey)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(cryptRndKey)

            _result = "OK"

            Return mr

        End Function

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> if the HSM is not in the authorized state.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As Message.MessageResponse
            If _result = "OK" Then
                Dim mr As New MessageResponse
                mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
                Return mr
            Else
                Return Nothing
            End If
        End Function

    End Class

End Namespace


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
    ''' Generates a random PIN of 4 to 12 digits.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the JA Racal command.
    ''' </remarks>
    <ThalesCommandCode("JA", "JB", "", "Generates a random PIN of 4 to 12 digits")> _
    Public Class GenerateRandomPIN_JA
        Inherits AHostCommand

        Private _acct As String
        Private _pinLen As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the JA message parsing fields.
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
                _acct = kvp.Item("Account Number")
                _pinLen = kvp.ItemOptional("PIN Length")
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

            If _pinLen = "" Then
                _pinLen = Convert.ToString(Convert.ToInt32(Core.Resources.GetResource(Core.Resources.CLEAR_PIN_LENGTH)))
            End If

            Dim PIN As String = GetRandomPIN(Convert.ToInt32(_pinLen))
            Dim PINCrypt As String = EncryptPINForHostStorage(PIN)

            Log.Logger.MinorInfo("Clear PIN: " + PIN)
            Log.Logger.MinorInfo("Crypt PIN: " + PINCrypt)
            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(PINCrypt)

            Return mr

        End Function

    End Class

End Namespace

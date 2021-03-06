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

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Returns HSM status information.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the Racal HSM status command.
    ''' </remarks>
    <ThalesCommandCode("NO", "NP", "", "Returns HSM status information")> _
    Public Class HSMStatus_NO
        Inherits AHostCommand

        Private _modeFlag As String

        Public Sub New()
            ReadXMLDefinitions()
        End Sub

        ''' <summary>
        ''' Parses the command message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            XML.MessageParser.Parse(msg, XMLMessageFields, kvp, XMLParseResult)
            If XMLParseResult = ErrorCodes.ER_00_NO_ERROR Then
                _modeFlag = kvp.Item("Mode Flag")
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
            mr.AddElement(Core.ErrorCodes.ER_00_NO_ERROR)
            If _modeFlag = "00" Then
                mr.AddElement("3")
                mr.AddElement("1")
                mr.AddElement(Convert.ToInt32(Core.Resources.GetResource(Core.Resources.MAX_CONS)).ToString())
                mr.AddElement(Convert.ToString(Core.Resources.GetResource(Core.Resources.FIRMWARE_NUMBER)))
                mr.AddElement("0")
                mr.AddElement(Convert.ToString(Core.Resources.GetResource(Core.Resources.DSP_FIRMWARE_NUMBER)))
            End If
            Return mr
        End Function

    End Class

End Namespace

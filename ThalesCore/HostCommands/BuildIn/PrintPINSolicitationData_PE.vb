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
    ''' Prints PIN solicitation data.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the PE Thales command. The implementation does not
    ''' perform any processing.
    ''' </remarks>
    <ThalesCommandCode("PE", "PF", "PZ", "Prints PIN solicitation data")> _
    Public Class PrintPINSolicitationData_PE
        Inherits NoImplementation

        ''' <summary>
        ''' Internal initialization method.
        ''' </summary>
        ''' <remarks>
        ''' This method provides specific implementation of the authorized and related I/O flags.
        ''' </remarks>
        Public Overrides Sub InitFields()
            NeedsAuthorizedState = True
            HasRelatedIO = True
        End Sub

    End Class

End Namespace



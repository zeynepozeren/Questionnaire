<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Questionaire._Default" 
    %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="Panel1" runat="server" BackColor="#FFBB77">
        <asp:Label ID="Label2" runat="server"></asp:Label>
        </asp:Panel>
     <asp:Panel ID="TableContainer" runat="server" style="margin-bottom: 23px" BackColor="#FFE2A8">
                        <asp:Table ID="Table1" runat="server" HorizontalAlign="Center">
                        </asp:Table>
                    </asp:Panel>
                <asp:Panel ID="Panel3" runat="server" HorizontalAlign="Center">
                    <asp:Button ID="Button1" runat="server" Font-Bold="True" OnClick="Button1_Click" Text="SUBMIT" />
                   
                </asp:Panel>
   

    </asp:Content>

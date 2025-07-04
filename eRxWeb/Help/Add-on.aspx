<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPageNew.master" AutoEventWireup="true" Inherits="eRxWeb.Help_Add_on" Title="Untitled Page" CodeBehind="Add-on.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
   
    <div class="row">
        <asp:Repeater ID="Repeater1" runat="server"  >
            <ItemTemplate>
                <div class="col-sm-12 col-md-4 ">
                    <div class="addon-tile">

                        <img id="x" runat="server" expand='<%# ObjectExtension.ToEvalEncode(Eval("ExpandedFile")) %>'  src='<%# ObjectExtension.ToEvalEncode(Eval("FileName")) %>' />
                        <div class="overlay">
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <button type="button" id="triggerAddOnModal" style="display: none;" data-toggle="modal" data-target="#addOnModal"></button>

    <!-- AddOn Modal -->
    <div id="addOnModal" class="modal fade" role="dialog">
        <div class="modal-dialog" style="width: 90%;">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <img id="addOnModalContent" width="100%" src="Images/AddOnExpand.PNG" />
                </div>
            </div>

        </div>
    </div>
    <script type="text/javascript">
        $('.addon-tile').click(function () {
            var src = $(this).find('img').attr('expand');
            $('#addOnModalContent').attr('src', src); 
            $('#addOnModal').modal();
        });
    </script>
</asp:Content>



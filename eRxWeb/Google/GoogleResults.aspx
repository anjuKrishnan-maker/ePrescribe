<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.GoogleResults" Codebehind="GoogleResults.aspx.cs" %>

<%@ Register Src="../Controls/GoogleSearch.ascx" TagName="GoogleSearch" TagPrefix="ePrescribe" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>eRxNow Allscripts/Google - Physician Custom Search Engine</title>
</head>
<body>
<!-- Google CSE Search Box Begins -->
<form id="searchbox_007346535748368836231:sbx1cfoc2pg" action="GoogleResults.aspx"> 
  <input type="hidden" name="cx" value="007346535748368836231:sbx1cfoc2pg" />
  <input id="q" name="q" type="text" value="<%=Request.QueryString["q"]%>"/>
  <input type="submit" name="sa" value="Search"  style="height: 20px; border: 1px solid #7e9db9; padding: 0px;"/>
  <input type="hidden" name="cof" value="FORID:9" />
</form> 

<script type="text/javascript">

(function() { 
   var f = document.getElementById('searchbox_007346535748368836231:sbx1cfoc2pg'); 
   if (f && f.q) { 
      var q = f.q;
      var n = navigator; 
      var l = location; 
      if (n.platform == 'Win32') { 
         q.style.cssText = 'border: 1px solid #7e9db9; padding: 0px;'; 
      } 
      var b = function() { 
           skipcycle=false;
           if (q.value == '') { 
              q.style.background = '#FFFFFF url(images/searchbox.gif) left no-repeat'; 
           } 
      }; 
      var f = function() { 
           skipcycle=true;
           q.style.background = '#ffffff'; }; 
           q.onfocus = f; q.onblur = b; 
           if (!/[&?]q=[^&]/.test(l.search)) { 
              b(); 
           } 
       } 
   }
)(); 
</script>


<!-- Google Search Result Snippet Begins -->
<div id="results_007346535748368836231%3Asbx1cfoc2pg">&nbsp;</div>
<script type="text/javascript">
  var googleSearchIframeName = "results_007346535748368836231%3Asbx1cfoc2pg";
  var googleSearchFormName = "searchbox_007346535748368836231%3Asbx1cfoc2pg";
  var googleSearchFrameWidth = 600;
  var googleSearchFrameborder = 0;
  var googleSearchDomain = "www.google.com";
  var googleSearchPath = "/cse";
</script>
<script type="text/javascript" src="http://www.google.com/afsonline/show_afs_search.js"></script>
<!-- Google Search Result Snippet Ends -->
</body>
</html>

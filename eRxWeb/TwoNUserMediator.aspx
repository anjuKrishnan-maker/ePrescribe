<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TwoNUserMediator.aspx.cs" Inherits="eRxWeb.TwoNUserMediator" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <meta http-equiv="x-ua-compatible" content="IE=edge" />
    <title>Veradigm ePrescribe - User Mediator</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
        }

        body {
            display: flex;
            min-height: 100vh;
            flex-direction: column;
            margin: 0;
        }

        #main {
            display: flex;
            flex: 1;
        }

            #main > article {
                flex: 1;
                order: 1;
                text-align: center;
            }

        .message {
            font-size: 3.312rem;
        }

        #main > nav,
        #main > aside {
            flex: 0 0 20vw;
        }

        header, footer {
            background: #2F2257;
            height: 20vh;
        }

        header, footer, article {
            padding: 1em;
        }
    </style>
</head>
<body>
    <header>
        <img src="images/Allscripts/Allscripts_Logo.jpg" alt="Allscripts" />
    </header>
    <div id="main">
        <article>
            <div class="message">Almost there!</div>
        </article>
    </div>
    <footer></footer>

</body>
</html>

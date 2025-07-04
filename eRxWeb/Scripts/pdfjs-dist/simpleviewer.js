'use strict';

function PDFSet(url,containerId) {
    var progressDiv = document.querySelector("#progress");
    progressDiv.classList.remove('.hide');
    // The workerSrc property shall be specified.
    //
    pdfjsLib.GlobalWorkerOptions.workerSrc =
        '/scripts/pdfjs-dist/build/pdf.worker.js';

    // Some PDFs need external cmaps.
    //
    var CMAP_URL = '/scripts/pdfjs-dist/cmaps/';
    var CMAP_PACKED = true;

    var DEFAULT_URL = '../../web/compressed.tracemonkey-pldi-09.pdf';
    var SEARCH_FOR = ''; // try 'Mozilla';

    var container = document.getElementById(containerId);

    // (Optionally) enable hyperlinks within PDF files.
    var pdfLinkService = new pdfjsViewer.PDFLinkService();

  //  pdfjsViewer.ProgressBar('progress');
    var pdfViewer = new pdfjsViewer.PDFViewer({
        container: container,
        linkService: pdfLinkService,
    });
    pdfLinkService.setViewer(pdfViewer);

    // (Optionally) enable find controller.
    var pdfFindController = new pdfjsViewer.PDFFindController({
        pdfViewer: pdfViewer,
    });
    pdfViewer.setFindController(pdfFindController);

    container.addEventListener('pagesinit', function () {
        // We can use pdfViewer now, e.g. let's change default scale.
        pdfViewer.currentScaleValue = 'page-width';

        if (SEARCH_FOR) { // We can try search for things
            pdfFindController.executeCommand('find', { query: SEARCH_FOR });
        }
    });
    pdfLinkService.externalLinkTarget = 1;
    // Loading document.
    pdfjsLib.getDocument({
        url: "/api/PdfStream/GetPdfContent/?url=" + url,// '/Content/Bwm8Mev6sAUVLZcfUOqDKQ2.pdf',
        cMapUrl: CMAP_URL,
        cMapPacked: CMAP_PACKED,
    }, false, null, function (progress) {
        var percent_loaded = (progress.loaded / progress.total) * 100;
        console.log(percent_loaded);
    }).then(function (pdfDocument) {
        // Document loaded, specifying document for the viewer and
        // the (optional) linkService.
        pdfViewer.setDocument(pdfDocument);
        
        progressDiv.classList.add("hide");
        pdfLinkService.setDocument(pdfDocument, null);
    });
}

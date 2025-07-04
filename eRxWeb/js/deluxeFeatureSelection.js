
function deluxeFeatureInit() {
    
    var fromPage = "";
    if (window.location.search) {
        fromPage = getQueryVariable('From');
    }
    
    var ModuleSubscribed = "";
    var isCompulsoryBasicPilotValue = "";
    if (fromPage == "Menu") {
        ModuleSubscribed = "deluxeepcsepa2017";
        isCompulsoryBasicPilotValue = "False";
    }
    else {
        ModuleSubscribed = document.getElementById("ctl00_ContentPlaceHolder1_HiddenFieldModuleSubscribed").value;
        isCompulsoryBasicPilotValue = document.getElementById("ctl00_ContentPlaceHolder1_HiddenFieldIsCompulsoryBasicPilot").value;
    }

    //Get Data From JSON
    var modulesData = document.getElementById("ctl00_ContentPlaceHolder1_hdnFldJsonData").value;
    var modulesDataJson = jQuery.parseJSON(modulesData);
    
    
    isCompulsoryBasicPilot = (isCompulsoryBasicPilotValue.toUpperCase() === 'TRUE') ? true : false;
    InitializeCompulsoryBasicUI(isCompulsoryBasicPilot);

    PopulateFeaturePrice(modulesDataJson);
     InitializeSelectionUI(ModuleSubscribed);
}
function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) { return pair[1]; }
    }
    return (false);
}
var isCompulsoryBasicPilot = false;

//Hold Price of Available Features
var FeaturePrice = {
    DeluxeEPA: 0,
    DeluxeEPALogRx: 0,
    DeluxeEpcsEpa: 0,
    DeluxeEpcsEpaLogRx: 0,
    CompulsoryBasic: 0
};

var selectedFeature = ''; //Feature Selected in UI

function InitializeCompulsoryBasicUI(isCompulsoryBasicRequired) {
   
    if (isCompulsoryBasicRequired) {
        $('#chkCompulsoryBasic').show();
    }
    else {
        $('#deluxeOnlyHeader').show();
    }
}

//Set Initial Feature
function InitializeSelectionUI(ModuleSubscribed) {
    try {
        switch (ModuleSubscribed.toUpperCase().trim()) {
            case 'COMPULSORYBASIC': setCompulsoryBasic();
                break;
            case 'DELUXEEPA': setDeluxeEpa();
                break;
            case 'DELUXEEPALOGRX': setDeluxeEpaLogRx();
                break;
            case 'DELUXEEPCSEPA': 
            case 'DELUXEEPCSEPA2017': setDeluxeEpcsEpa();
                break;
            case 'DELUXEEPCSEPALOGRX': 
            case 'DELUXEEPCSEPALOGRX2017': setDeluxeEpcsEpaLogRx();
                break;
            default: setNoFeature(); break;
        }
    }
    catch(err) {
        setNoFeature();
    }
}

//Set the price box content present on the right side of the screen
function DisplayPriceBlock(blockID) {
    if(blockID === 'None') {
        $('#divPriceBlock').hide();
    }
    else {
        $('#divPriceBlock').show();
        //Hide all Content and then show only thats required
        $('#contentCompulsoryBasic').hide();
        $('#contentDeluxeEpa').hide();
        $('#contentDeluxeEpaLogRx').hide();
        $('#contentDeluxeEpcsEpa').hide();
        $('#contentDeluxeEpcsEpaLogRx').hide();
        switch (blockID) {
            case 'CompulsoryBasic':
                $('#contentCompulsoryBasic').show();
                break;
            case 'DeluxeEpa':
                $('#contentDeluxeEpa').show();
                break;
            case 'DeluxeEpaLogRx':
                $('#contentDeluxeEpaLogRx').show();
                break;
            case 'DeluxeEpcsEpa':
            case 'DeluxeEpcsEpa2017':
                $('#contentDeluxeEpcsEpa').show();
                break;
            case 'DeluxeEpcsEpaLogRx':
            case 'DeluxeEpcsEpaLogRx2017':
                $('#contentDeluxeEpcsEpaLogRx').show();
                break;            
        }
    }
}

//Set the FeaturePrice object and related html changes
function PopulateFeaturePrice(modulesDataJson) {
    for (var i = 0; i < modulesDataJson.length; i++) {
        switch (modulesDataJson[i].Module.toUpperCase())
        {
            case 'COMPULSORYBASIC': FeaturePrice.CompulsoryBasic = modulesDataJson[i].Price;
                $('#priceCompulsoryBasic').html(FeaturePrice.CompulsoryBasic);
                break;
            case 'DELUXEEPA': FeaturePrice.DeluxeEPA = modulesDataJson[i].Price;
                $('#priceDeluxeEpa').html(FeaturePrice.DeluxeEPA);
                break;
            case 'DELUXEEPALOGRX': FeaturePrice.DeluxeEPALogRx = modulesDataJson[i].Price;
                $('#priceDeluxeEpaLogRx').html(FeaturePrice.DeluxeEPALogRx);
                break;
            case 'DELUXEEPCSEPA2017': FeaturePrice.DeluxeEpcsEpa = modulesDataJson[i].Price;
                $('#priceDeluxeEpcsEpa').html(FeaturePrice.DeluxeEpcsEpa);
                break;
            case 'DELUXEEPCSEPALOGRX2017': FeaturePrice.DeluxeEpcsEpaLogRx = modulesDataJson[i].Price;
                $('#priceDeluxeEpcsEpaLogRx').html(FeaturePrice.DeluxeEpcsEpaLogRx);
                break;
        }
    }
}

//Validate on Continue Button Click
function Validate() {
    clearWarningMessage();
    if ($('#chkDeluxeEpa').find('input').is(':checked') === false && $('#chkCompulsoryBasic').find('input').is(':checked') === false) {
        $("#warningMessage").html("Please select a feature to continue");
        return false;
    }
    var HiddenFieldSelectedFeature = document.getElementById("ctl00_ContentPlaceHolder1_HiddenFieldSelectedFeature");
    HiddenFieldSelectedFeature.value = selectedFeature;
    return true;
}

function clearWarningMessage() {
    $("#warningMessage").html("");
}

//UI Changes for No Feature Selected
function setNoFeature() {
    clearWarningMessage();
    selectedFeature = '';
    $('#chkCompulsoryBasic').find('input').attr('checked', false);
    $('#chkDeluxeEpa').find('input').attr('checked', false);
    $('#chkDeluxeEpcsEpa').hide();
    $('#chkDeluxeEpcsEpa').find('input').attr('checked', false);
    $('#chkLogRx').hide();
    $('#chkLogRx').find('input').attr('checked', false);
    DisplayPriceBlock('None');
}

//UI Changes for Compulsory Basic
function setCompulsoryBasic() {
    clearWarningMessage();
    selectedFeature = 'CompulsoryBasic';
    $('#chkCompulsoryBasic').find('input').attr('checked', true);
    $('#chkDeluxeEpa').find('input').attr('checked', false);
    $('#chkDeluxeEpcsEpa').hide();
    $('#chkDeluxeEpcsEpa').find('input').attr('checked', false);
    $('#chkLogRx').hide();
    $('#chkLogRx').find('input').attr('checked', false);
    DisplayPriceBlock('CompulsoryBasic');
}

//UI Changes for DeluxeEpa
function setDeluxeEpa() {
    clearWarningMessage();
    selectedFeature = 'DeluxeEpa';
    $('#chkCompulsoryBasic').find('input').attr('checked', false);
    $('#chkDeluxeEpa').find('input').attr('checked', true);
    $('#chkDeluxeEpcsEpa').show();
    $('#chkDeluxeEpcsEpa').find('input').attr('checked', false);
    $('#chkLogRx').show();
    $('#chkLogRx').find('input').attr('checked', false);
    DisplayPriceBlock('DeluxeEpa');
}

//UI Changes for DeluxeEpaLogRx
function setDeluxeEpaLogRx() {
    clearWarningMessage();
    selectedFeature = 'DeluxeEpaLogRx';
    $('#chkCompulsoryBasic').find('input').attr('checked', false);
    $('#chkDeluxeEpa').find('input').attr('checked', true);
    $('#chkDeluxeEpcsEpa').show();
    $('#chkDeluxeEpcsEpa').find('input').attr('checked', false);
    $('#chkLogRx').show();
    $('#chkLogRx').find('input').attr('checked', true);
    DisplayPriceBlock('DeluxeEpaLogRx');
}

//UI Changes for DeluxeEpcsEpa
function setDeluxeEpcsEpa() {
    clearWarningMessage();
    selectedFeature = 'DeluxeEpcsEpa2017';
    $('#chkCompulsoryBasic').find('input').attr('checked', false);
    $('#chkDeluxeEpa').find('input').attr('checked', true);
    $('#chkDeluxeEpcsEpa').show();
    $('#chkDeluxeEpcsEpa').find('input').attr('checked', true);
    $('#chkLogRx').show();
    $('#chkLogRx').find('input').attr('checked', false);
    DisplayPriceBlock('DeluxeEpcsEpa');
}

//UI Changes for DeluxeEpcsEpaLogRx
function setDeluxeEpcsEpaLogRx() {
    clearWarningMessage();
    selectedFeature = 'DeluxeEpcsEpaLogRx2017';
    $('#chkCompulsoryBasic').find('input').attr('checked', false);
    $('#chkDeluxeEpa').find('input').attr('checked', true);
    $('#chkDeluxeEpcsEpa').show();
    $('#chkDeluxeEpcsEpa').find('input').attr('checked', true);
    $('#chkLogRx').show();
    $('#chkLogRx').find('input').attr('checked', true);
    DisplayPriceBlock('DeluxeEpcsEpaLogRx');
}

//Event Registration For UI Checkbox Clicks
$(document).ready(function () {
    //Checkbox - Compulsory Basic Clicked
    $('#chkCompulsoryBasic').click(function () {
        $('#chkDeluxeEpa').find('input').attr('checked', false);
        $('#chkDeluxeEpcsEpa').find('input').attr('checked', false);
        $('#chkLogRx').find('input').attr('checked', false);
        determineFeature();
    });

    //Checkbox - DeluxeEpa Clicked
    $('#chkDeluxeEpa').click(function () {
        $('#chkCompulsoryBasic').find('input').attr('checked', false);
        if ($('#chkDeluxeEpa').find('input').attr('checked')) {
            $('#chkLogRx').find('input').attr('checked', true);
        }
        determineFeature();
    });

    //Checkbox - DeluxeEpcsEpa Clicked
    $('#chkDeluxeEpcsEpa').click(function () {
        $('#chkCompulsoryBasic').find('input').attr('checked', false);
        determineFeature();
    });

    //Checkbox - Sponsored Messages Clicked
    $('#chkLogRx').click(function () {
        $('#chkCompulsoryBasic').find('input').attr('checked', false);
        determineFeature();
    });
});

function determineFeature() {
    var chkCompulsoryBasic = $('#chkCompulsoryBasic').find('input').is(':checked');
    var chkDeluxeEpa = $('#chkDeluxeEpa').find('input').is(':checked');
    var chkDeluxeEpcsEpa = $('#chkDeluxeEpcsEpa').find('input').is(':checked');
    var chkLogRx = $('#chkLogRx').find('input').is(':checked');

    if (chkCompulsoryBasic) {
        setCompulsoryBasic();
    }
    else {
        if (chkDeluxeEpa) {
            if (!chkDeluxeEpcsEpa && !chkLogRx) {
                setDeluxeEpa();
            }
            else if (!chkDeluxeEpcsEpa && chkLogRx) {
                setDeluxeEpaLogRx();
            }
            else if (chkDeluxeEpcsEpa && !chkLogRx) {
                setDeluxeEpcsEpa();
            }
            else {
                setDeluxeEpcsEpaLogRx();
            }
        }
        else {
            setNoFeature();
        }
    }
}


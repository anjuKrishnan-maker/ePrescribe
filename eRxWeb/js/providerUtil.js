function SpecialtyMedEnrollmentAlert()
{
    $.ajax({
        type: 'POST',
        url: '/JsonGateway.aspx/GetSpecialtyMedEnrollment',
        data: null,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (specMedEnrollment) {
            try {
                if (specMedEnrollment.d) {
                    var specMedEnr = JSON.parse(specMedEnrollment.d);
                    prompt("Specialty Enrollment: " + specMedEnr["IsProviderEnrolledInSpecialtyMed"] + "\n"
                        + "Provider Npi: " + specMedEnr["NPIForSpecialtyMed"] + "\n"
                        + "Provider UserId: " + specMedEnr["UserId"] + "\n", "NPI = " + specMedEnr["NPIForSpecialtyMed"] + " UserId = " + specMedEnr["UserId"]);
                }
            }
            catch (err) {
                alert("Error getting enrollment: " + err);
            }
        }
    });
}

function CheckSpecialtyMedEnrollment() {
    $.ajax({
        type: 'POST',
        url: '/JsonGateway.aspx/CheckSpecialtyMedEnrollment',
        data: null,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    });
}
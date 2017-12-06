

//Trigger when Check in submit button clicked.
$('body').on("submit", "#btnActivityCheckIn", function (event) {

    if ($('#txtIsActivityCheckInSubmitButtonClicked').val() == "false") {
        var isSuccess = false;
        bootbox.prompt({
            title: "Please enter your password to confirm your CHECK-IN action:",
            inputType: 'password',
            callback: function (result) {

                if (result == "" || result == null) {
                    event.preventDefault();
                    //Stop submit
                    $('#txtIsActivityCheckInSubmitButtonClicked').val('false');
                    bootbox.alert('Invalid Input! Please try again.');
                }
                else {
                    //submit the form                  
                    isSuccess = true;
                    $('#txtActivityCheckInPassword').val(result);
                    $('#txtIsActivityCheckInSubmitButtonClicked').val('true');
                    $("#btnActivityCheckIn").submit();
                }
            }
        });

        if (isSuccess == false) {
            event.preventDefault();
        }
    }
});

//Trigger when Authorization submit button clicked.
$('body').on("submit", "#btnActivityAuthorization", function (event) {

    if ($('#txtIsActivityAuthorizationSubmitButtonClicked').val() == "false") {
        var isSuccess = false;
        bootbox.prompt({
            title: "Please enter your password to confirm your AUTHORIZATION action:",
            inputType: 'password',
            callback: function (result) {

                if (result == "" || result == null) {
                    event.preventDefault();
                    //Stop submit
                    $('#txtIsActivityAuthorizationSubmitButtonClicked').val('false');
                    bootbox.alert('Invalid Input! Please try again.');
                }
                else {
                    //Submit the form                  
                    isSuccess = true;
                    $('#txtActivityAuthorizationPassword').val(result);
                    $('#txtIsActivityAuthorizationSubmitButtonClicked').val('true');
                    $("#btnActivityAuthorization").submit();
                }
            }
        });

        if (isSuccess == false) {
            event.preventDefault();
        }
    }
});

//Trigger when RAMP OUT submit button clicked.
$('body').on("submit", "#btnActivityRampOut", function (event) {

    if ($('#txtIsActivityRampOutSubmitButtonClicked').val() == "false") {
        var isSuccess = false;
        bootbox.prompt({
            title: "Please enter your password to confirm your RAMP OUT action:",
            inputType: 'password',
            callback: function (result) {

                if (result == "" || result == null) {
                    event.preventDefault();
                    //Stop submit
                    $('#txtIsActivityRampOutSubmitButtonClicked').val('false');
                    bootbox.alert('Invalid Input! Please try again.');
                }
                else {
                    //Submit the form                  
                    isSuccess = true;
                    $('#txtActivityRampOutPassword').val(result);
                    $('#txtIsActivityRampOutSubmitButtonClicked').val('true');
                    $("#btnActivityRampOut").submit();
                }
            }
        });

        if (isSuccess == false) {
            event.preventDefault();
        }
    }
});

//Trigger when RAMP IN submit button clicked.
$('body').on("submit", "#btnActivityRampIn", function (event) {

    if ($('#txtIsActivityRampInSubmitButtonClicked').val() == "false") {
        var isSuccess = false;
        bootbox.prompt({
            title: "Please enter your password to confirm your RAMP IN action:",
            inputType: 'password',
            callback: function (result) {

                if (result == "" || result == null) {
                    event.preventDefault();
                    //Stop submit
                    $('#txtIsActivityRampInSubmitButtonClicked').val('false');
                    bootbox.alert('Invalid Input! Please try again.');
                }
                else {
                    //Submit the form                  
                    isSuccess = true;
                    $('#txtActivityRampInPassword').val(result);
                    $('#txtIsActivityRampInSubmitButtonClicked').val('true');
                    $("#btnActivityRampIn").submit();
                }
            }
        });

        if (isSuccess == false) {
            event.preventDefault();
        }
    }
});

//Trigger when FLIGHT LOG submit button clicked.
$('body').on("submit", "#btnFlightLogs", function (event) {
   
    if ($('#txtIsActivityFlightLogSubmitButtonClicked').val() == "false") {
        var isSuccess = false;
        bootbox.prompt({
            title: "Please enter your password to confirm your FLIGHT LOG action:",
            inputType: 'password',
            callback: function (result) {

                if (result == "" || result == null) {
                    event.preventDefault();
                    //Stop submit
                    $('#txtIsActivityFlightLogSubmitButtonClicked').val('false');
                    bootbox.alert('Invalid Input! Please try again.');
                }
                else {
                    //Submit the form                  
                    isSuccess = true;
                    $('#txtFlightLogPassword').val(result);
                    $('#txtIsActivityFlightLogSubmitButtonClicked').val('true');
                    $("#btnFlightLogs").submit();
                }
            }
        });

        if (isSuccess == false) {
            event.preventDefault();
        }
    }
});

//Triggered whenever Evaluation submit button is clicked
$('.FTDAndGFSchedulingContainer').on("submit", "#btnEvaluationTemplate", function (event) {
    debugger;
    //collect and add checked value into a single space / text field
    $('#EvaluationItem').val('');
    var EvaluationItemList = '';
    
    //Get value to be posted
    var score = $('#txtOverallGrade').val();
    var overAllGradeId = $('#OverallGradeId').val();

    var traineeId = $('#txtEditTraineeId').val();
    var lessonId = $('#txtEditLessonId').val();
    var remark = $('#txtRemark').val();
    var fylingFTDScheduleId = $('#txtEventID').text();
    var TimeIn = $('#txtTimeIn').val();
    var TimeOut = $('#txtTimeOut').val();
    var FlightTime = $('#txtFliTime').val();
    var FlightDate = $('#txtEditDate').val();

    var categoryId = '';
    var ItemId = '';
    var ScoreLevelId = '';
    var numberOfClickedEvaluationItems = 0;

    $('#evaluation-container :checked').each(function () {
        numberOfClickedEvaluationItems = numberOfClickedEvaluationItems + 1;
        var Ids = $(this).val();
        var IdsArray = Ids.split('~');

        categoryId = IdsArray[0];
        ItemId = IdsArray[1];
        ScoreLevelId = IdsArray[2];
        if (ItemId != 'undefined' && ScoreLevelId != 'undefined' && ItemId != null && ScoreLevelId != null && ItemId != '' && ScoreLevelId != '')
            EvaluationItemList = EvaluationItemList + ItemId + "-" + ScoreLevelId + "~";
        //alert('categoryId = ' + categoryId + ' ItemId=' + ItemId + ' ScoreLevelId=' + ScoreLevelId);
    });

    var numberOfRenderedEvaluationItems = $('#txtnumberOfItems').val();

    var isEvaluationSucceed = true;
    event.preventDefault();

    var evaluationData = {
        'evaluationTemplateItems': EvaluationItemList,
        'overAllGradeId': overAllGradeId,
        'traineeId': traineeId,
        'lessonId': lessonId,
        'remark': remark,
        'fylingFTDScheduleId': fylingFTDScheduleId,
        'TimeIn': TimeIn,
        'TimeOut': TimeOut,
        'FlightTime': FlightTime,
        'FlightDate': FlightDate,
    }
    debugger;
    if (numberOfClickedEvaluationItems < parseInt(numberOfRenderedEvaluationItems)) {

            
            bootbox.confirm("You did not evaluate all items. Are you sure you want to <b>continue</b>?", function (result) {
         

                    $.ajax({
                        url: "/TraineeEvaluationTemplate/SaveTraineeEvaluationTemplateItems",
                        data: evaluationData,
                        type: 'POST',
                        success: function (result) {

                            if (result.IsSuccess) {
                                bootbox.alert(result.message);
                            }
                            else {
                                bootbox.alert(result.message);
                            }
                        },
                        error: function () {
                        }
                    });
               
            });

    }
    else {
        

            $.ajax({
                url: "/TraineeEvaluationTemplate/SaveTraineeEvaluationTemplateItems",
                data: evaluationData,
                type: 'POST',
                success: function (result) {

                    if (result.IsSuccess) {
                        bootbox.alert(result.message);
                    }
                    else {
                        bootbox.alert(result.message);
                    }
                },
                error: function () {
                }
            });

    }
});





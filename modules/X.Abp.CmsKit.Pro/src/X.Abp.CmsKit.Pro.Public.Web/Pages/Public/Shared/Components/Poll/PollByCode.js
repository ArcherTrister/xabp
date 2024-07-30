(function ($) {

    var l = abp.localization.getResource("CmsKit");

    abp.widgets.CmsPollByCode = function ($widget) {

        var widgetManager = $widget.data('abp-widget-manager');
        var $pollArea = $widget.find(".cms-poll-area");
        var pollId = $pollArea.attr("data-id");
        var code = $pollArea.attr("data-widget");

        var $singleSelectionOptions = $widget.find('#SingleSelectionOptions');
        var $multipleSelectionOptions = $widget.find('#MultipleSelectionOptions');
        var $getResultButton = $widget.find('#CmsPollFooterGetResultButton');
        var $voteButton = $widget.find("#CmsPollFooterVoteButton");
        var $returnVotingButton = $widget.find('#CmsPollFooterReturnVotingButton');
        var $resultSection = $widget.find('#CmsPollResultSection');
        var $votingSection = $widget.find('#CmsPollVotingSection');

        function getFilters() {
            return {
                code: code,
            };
        }

        function init() {

            $getResultButton.click(function () {

                $($resultSection[0]).show();
                $($returnVotingButton[0]).show();

                $($votingSection[0]).hide();
                $($getResultButton[0]).hide();
                $($voteButton[0]).hide();
                    
            });

            $returnVotingButton.click(function () {

                $($resultSection[0]).hide();
                $($returnVotingButton[0]).hide();

                $($votingSection[0]).show();
                $($getResultButton[0]).show();
                $($voteButton[0]).show();
                
            });

            $voteButton.on('click', '', function (e) {
                
                var selectedOptions = [];
                
                if ($multipleSelectionOptions.length > 0) {
                    var optionInputs = $multipleSelectionOptions.find('input');

                    for (var i = 0; i < optionInputs.length; i++) {

                        if($(optionInputs[i]).is(":checked")){
                            var optionId = $(optionInputs[i]).data("option-id");
                            selectedOptions.push(optionId);
                        }
                    }
                }
                else {
                    var optionInputs = $singleSelectionOptions.find('input');

                    for (var i = 0; i < optionInputs.length; i++) {
                        
                        if($(optionInputs[i]).is(":checked")){
                            var optionId = $(optionInputs[i]).data("option-id");
                            selectedOptions.push(optionId);
                            break;
                        }
                    }
                }
                
                if (selectedOptions.length < 1){
                    abp.message.warn(l('YouHaventSelectedAnyOption'));
                    return;
                }
                
                var submitPollInput = {
                    pollOptionIds: selectedOptions
                };

                x.cmsKit.public.polls.pollPublic.submitVote(pollId, submitPollInput).then(function () {
                    abp.notify.success(l('VotedSuccessfully'));
                    widgetManager.refresh($widget);
                });
            });
        }

        return {
            init: init,
            getFilters: getFilters
        }
    };




})(jQuery);

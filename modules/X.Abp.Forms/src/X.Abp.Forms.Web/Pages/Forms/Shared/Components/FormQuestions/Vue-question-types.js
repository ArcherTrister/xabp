Vue.component('question-type', {
    template: `
        <div>
            <select v-model="selectedQuestionType" data-show-content="true" class="select form-control" name="QuestionType">
            </select>
        </div>`,
    data() {
        return {
            selectedQuestionType: -1,
            options: [
                {
                    id: 1,
                    text: abp.localization.getResource('Forms')("QuestionType:ShortText")
                },
                // {
                //     id: 2,
                //     text: '<i class="fa fa-align-left select2-option-icon"></i> ' + abp.localization.getResource('Forms')("QuestionType:ParagraphText")
                // },
                {
                    id: 3,
                    text: abp.localization.getResource('Forms')("QuestionType:ChoiceMultiple")
                },
                {
                    id: 4,
                    text: abp.localization.getResource('Forms')("QuestionType:Checkbox")
                },
                {
                    id: 5,
                    text: abp.localization.getResource('Forms')("QuestionType:DropdownList")
                }
            ]
        }
    },
    props: {
        questionTypeSelected: {
            default: null
        }
    },
    model: {
        prop: 'questionTypeSelected',
        event: 'changed:question_type'
    },
    computed: {},
    created() {
        this.l = abp.localization.getResource('Forms');
    },
    mounted() {
        const vm = this;
        const elem = this.$el.querySelector(".select");
        $(elem).select2({
            theme: "default",
            data: this.options,
            escapeMarkup: function (markup) {
                switch (markup) {
                    case abp.localization.getResource('Forms')("QuestionType:ShortText"): {
                        return '<i class="fa fa-stream select2-option-icon"></i> ' + markup;
                        break;
                    }
                    case abp.localization.getResource('Forms')("QuestionType:ChoiceMultiple"): {
                        return '<i class="fa fa-dot-circle select2-option-icon"></i> ' + markup;
                    }
                    case abp.localization.getResource('Forms')("QuestionType:Checkbox"): {
                        return '<i class="fa fa-check-square select2-option-icon"></i> ' + markup;
                    }
                    case abp.localization.getResource('Forms')("QuestionType:DropdownList"): {
                        return '<i class="fa fa-caret-square-down select2-option-icon"></i> ' + markup;
                    }
                    default: {
                        return markup;
                    }
                }

            }
        }).on('change', function () {
            if (vm.selectedQuestionType !== -1) {
                vm.$emit('changed:question_type', vm.selectedQuestionType, parseInt(this.value));
            }

            vm.selectedQuestionType = this.value;
        })
        $(elem).val(this.questionTypeSelected);
        $(elem).trigger('change');
    },
    methods: {
        typeChanged() {
            //deprecated-> will be removed since select2 is used instead of bootstrap-select
            this.$emit('changed:question_type', this.selectedQuestionType);
        }
    }
});
// Register the component globally.
Vue.component('ValidationProvider', VeeValidate.ValidationProvider);
Vue.component('ValidationObserver', VeeValidate.ValidationObserver);

(function () {
    const vuePage = new Vue({
        el: '#vue-form-view',
        data: {
            l: abp.localization.getResource('Forms'),
            formAppService: x.forms.forms.form,
            formResponseService: x.forms.responses.response,
            form: {},
            questions: [],
            editing: false,
            formId: null,
            email: null,
            extraProperties: {},
            isPreview: $('#myForm').attr('data-preview') === 'True'
        },
        mounted() {
            this.formId = this.$refs.form.value;
            this.initialize();
            // window.onbeforeunload = () => 'Are you sure you want to leave?'
        },
        methods: {
            async initialize() {
                try {
                    abp.ui.setBusy('#vue-form-view');
                    this.form = await this.formAppService.get(this.formId);

                    const formResponseId = localStorage.getItem('formResponseId');

                    if (this.form.canEditResponse && formResponseId != null) {
                        this.editing = true;
                        const response = await this.formResponseService.get(formResponseId);
                        this.email = response.email;

                        this.questions = await this.formResponseService.getQuestionsWithAnswers(formResponseId);
                        const notifyOptions = {
                            "positionClass": "toast-bottom-left",
                            "onclick": null,
                            "showDuration": "0",
                            "timeOut": 0,
                            "extendedTimeOut": 0,
                            "tapToDismiss": false
                        };
                        const toastrHtml = `<div>
                            <div class="row col">${this.l("Response:EditingResponseMsg")}</div>
                            <br />
                            <div class="row col"><button id="newResponseButton" class="btn btn-secondary">${this.l("Response:SubmitNewAnswer")}</button></div>
                        </div>`;
                        this.addEvent('click', '#newResponseButton', e => {
                            localStorage.removeItem('formResponseId');
                            localStorage.removeItem(this.formId);
                            this.editing = false;
                            location.reload();
                        });
                        abp.notify.info(toastrHtml, "", notifyOptions);

                    } else {
                        this.questions = await this.formAppService.getQuestions(this.formId);
                    }
                } catch (error) {
                    setTimeout(function () {
                        abp.message.error(error.message);
                    }, 500);
                } finally {
                    abp.ui.clearBusy();
                }

            },
            async submitAnswers(submittedForm) {
                const validationResult = await this.$refs.observer.validateWithInfo();
                if (!validationResult.isValid) {
                    return;
                }

                if (this.isPreview) {
                    return;
                }

                if (this.editing) {
                    await this.updateAnswers();
                    return;
                }

                let answers = JSON.parse(localStorage.getItem(this.formId)) || [];
                let postData = {
                    email: this.email,
                    answers: answers
                }

                try {
                    const result = await this.formResponseService.saveAnswers(this.formId, postData);
                    location.href = `${window.origin}/Forms/${result.id}/FormResponse`;
                } catch (error) {
                    setTimeout(function () {
                        abp.message.error(error.message);
                    }, 500);
                } finally {
                    localStorage.removeItem(this.formId);
                }
            },
            async updateAnswers() {
                try {
                    const formResponseId = localStorage.getItem('formResponseId');
                    let answers = JSON.parse(localStorage.getItem(this.formId)) || [];
                    let data = {
                        id: formResponseId,
                        formId: this.formId,
                        email: this.email,
                        answers: answers
                    };

                    let result = await this.formResponseService.updateAnswers(formResponseId, data);
                    localStorage.removeItem('formResponseId');
                    localStorage.removeItem(this.formId);
                    location.href = `${window.origin}/Forms/${result.id}/FormResponse`;
                } catch (error) {
                    setTimeout(function () {
                        abp.message.error(error.message);
                    }, 500);
                }
            },
            addEvent(event, selector, handler) {
                document.addEventListener(event, function (e) {
                    if (e.target.matches(selector + ', ' + selector + ' *')) {
                        handler.apply(e.target.closest(selector), arguments);
                    }
                }, false);
            },
            answerUpdated(data) {
                let answers = JSON.parse(localStorage.getItem(this.formId)) || [];

                if (data.length === 1) {
                    let itemIndex = answers.findIndex(x => x.questionId === data.questionId);
                    if (itemIndex === -1) {
                        answers.push({
                            questionId: data.questionId,
                            choiceId: data.answers[0].choiceId,
                            value: data.answers[0].value
                        });
                    } else {
                        answers = answers.map(x => (x.questionId === data.questionId) ? data : x);
                    }
                } else { //checkbox answers have same questionId
                    answers = answers.filter(e => {
                        return e.questionId !== data.questionId
                    });
                    data.answers.forEach(answer => {
                        answers.push({
                            questionId: data.questionId,
                            choiceId: answer.choiceId,
                            value: answer.value
                        });
                    });
                }

                localStorage.setItem(this.formId, JSON.stringify(answers));
            },
            updatedEmail($event) {
                this.email = $event.value;
            },
        },
        computed: {}
    });
})();

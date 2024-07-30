const vueResponseComponent = new Vue({
    el: '#vue-responses',
    data: {
        formId: location.pathname.split('/')[2],
        l: abp.localization.getResource('Forms'),
        formAppService: x.forms.forms.form,
        responseAppService: x.forms.responses.response,
        auth: abp.auth,
        otherChoice: String,
        isLoading: true,
        form: {
            id: '', 
            title: '', 
            description: ''
        },
        questions: [],
        response: {
            totalCount: 0,
            items: []
        },
        selectedResponse: {
            answers: []
        },
        selectedAnswersForResponse: [],
        selectedResponseIndex: 1,
        selectedResponseQuestions: [],
        selectedQuestionForResponses: {},
        selectedChoicesForResponse: [],
        groupedAnswers: {
            all: [],
            byQuestion: [],
            byQuestionMap: [],
            byResponse: [],
            byResponseMap: [],
            byQuestionThenResponse: [],
            byQuestionThenResponseMap: [],
        },
        horizontalBar_options: {
            responsive: true,
            lineTension: 1,
            scales: {
                xAxes: [{
                    ticks: {
                        min: 0,
                        stepSize: 1
                    }
                }]
            }
        },
    },
    async created() {
        const formItem = await this.formAppService.get(this.formId);
        this.initForm(formItem);
        await this.setResponse();
    },
    async mounted() {
        this.otherChoice = this.$refs.other.getAttribute('data-value');
        this.initialize();
        this.isLoading = false;
    },
    methods: {
        async setResponse(index = 1){
            const responseItem = await this.formAppService.getResponses(this.formId, { maxResultCount: 1, skipCount: index - 1 });
            this.initResponses(responseItem);
        },
        initForm(formItem) {
            this.form.id = formItem.id;
            this.form.title = formItem.title;
            this.form.description = formItem.description;
            this.questions = formItem.questions.map(obj => ({...obj, isDirty: false}));
            this.questions = _.orderBy(this.questions, 'index');
        },
        initResponses(responseItem) {
            this.response = responseItem
            if (this.response.totalCount > 0) {
                this.selectedResponse = this.response.items[0];
                this.selectedResponseQuestions = this.questions.filter(q => q.formId === this.selectedResponse.formId);
            }
        },
        downloadResponses() {
            window.location = abp.appPath + `api/forms/${this.form.id}/download-responses-csv?sorting=id%20asc`;
        },
        deleteResponses() {
            abp.message.confirm(this.l("Form:Responses:AllResponsesWillBeDeletedMsg"))
                .then(confirmed => {
                    if (confirmed) {
                        vueResponseComponent.formAppService.deleteAllResponsesOfForm(vueResponseComponent.form.id)
                            .then(_ => {
                                location.reload();
                            });
                    }
                });
        },
        deleteResponse() {
            if (!vueResponseComponent.selectedResponse.id) {
                return;
            }
            abp.message.confirm(this.l("Form:Responses:ResponseWillBeDeletedMsg"))
                .then(confirmed => {
                    if (confirmed) {
                        vueResponseComponent.responseAppService.delete(vueResponseComponent.selectedResponse.id)
                            .then(_ => {
                                location.reload();
                            });
                    }
                });
        },
        updateQuestionType(e) {
            this.handleSelectedQuestionResponses(e.currentTarget.value);
        },
        initialize() {
            const vm = this;
            let answerArray = [];
            const responseMap = new Map();
            this.response.items.forEach(rp => {
                answerArray.push(rp.answers);
                let rpAnswers = []
                rp.answers.forEach((item) => {
                    rpAnswers.push(item);
                });
                let collection = responseMap.get(rp.id);
                if (!collection) {
                    responseMap.set(rp.id, rpAnswers);
                } else {
                    collection = [...collection, rpAnswers];
                }
            });
            this.groupedAnswers.byResponseMap = responseMap;
            this.groupedAnswers.all = answerArray.flat();
            this.groupedAnswers.byQuestionMap = this.groupBy(this.groupedAnswers.all, r => r.questionId);

            const elem = this.$el.querySelector(".select2");
            $(elem).select2({
                theme: "default"
            }).on('change', function () {
                vm.selectedQuestionForResponses = vm.questions.find(q => q.id === this.value);
                vm.handleSelectedQuestionResponses(this.value);
            })
            if (this.questions.length > 0) {
                this.handleSelectedQuestionResponses(this.questions[0].id);
                $(elem).val(this.questions[0].id);
                $(elem).trigger('change');
            }
        },
        handleSelectedQuestionResponses(questionId) {
            this.selectedQuestionForResponses = this.questions.find(q => q.id === questionId);
            this.groupedAnswers.byQuestion = this.groupedAnswers.byQuestionMap.get(questionId) || new Map();
            this.selectedAnswersForResponse = this.getAnswersForResponse(this.groupedAnswers.byQuestion);
            this.groupedAnswers.byQuestionThenResponseMap = this.groupBy(this.groupedAnswers.byQuestion, r => r.formResponseId);
            this.selectedChoicesForResponse = this.getChoicesForResponse(this.selectedQuestionForResponses.choices, this.selectedQuestionForResponses.questionType);
        },
        getChoicesForResponse(choices, questionType) {
            let choiceArr = [];
            // if (questionType === 3) {
            choices.forEach(ch => {
                const chGroup = this.groupBy(this.groupedAnswers.byQuestion, ch => ch.choiceId);
                const responses = chGroup.get(ch.id) || [];
                const updatedChoice = {
                    ...ch,
                    responses: [...responses]
                }
                choiceArr.push(updatedChoice);
            })
            const selectedQuestion = this.selectedQuestionForResponses;
            const responseMapDifference = this.getDifferenceByKeyBetweenMaps(this.groupedAnswers.byResponseMap, this.groupedAnswers.byQuestionThenResponseMap);

            responseMapDifference.forEach((value, key) => {
                choiceArr.push({
                    questionId: selectedQuestion.id,
                    formResponseId: key,
                    isEmpty: true,
                    value: vueResponseComponent.l("Form:Responses:BlankQuestion"),
                    responses: [{formResponseId: key}]
                });
            });
            // } 
            // else if (questionType === 4) { //TODO: will be decided later
            //    
            // }

            return choiceArr;
        },
        getAnswersForResponse(filteredAnswers) {
            let answerArr = [];
            filteredAnswers.forEach(answer => {
                const answerGroup = this.groupBy(this.groupedAnswers.byQuestion, a => a.value);
                const responses = answerGroup.get(answer.value) || [];
                const updatedAnswer = {
                    ...answer,
                    responses: [...responses]
                }
                answerArr.push(updatedAnswer);
            });
            const selectedQuestion = this.selectedQuestionForResponses;
            this.groupedAnswers.byResponseMap.forEach((value, key) => {
                const existingAnswer = answerArr.find(q => q.formResponseId === key);
                if (!existingAnswer) {
                    answerArr.push({
                        questionId: selectedQuestion.id,
                        formResponseId: key,
                        isEmpty: true,
                        value: vueResponseComponent.l("Form:Responses:BlankQuestion"),
                        responses: [{formResponseId: key}]
                    })
                }
            });
            return answerArr;
        },
        getDifferenceByKeyBetweenMaps(map1, map2) {
            const map = new Map();
            map1.forEach((value, key) => {
                const existingMap = map2.get(key);
                if (!existingMap) {
                    map.set(key, value);
                }
            });
            return map;
        },
        groupBy(list, keyGetter) {
            const map = new Map();
            list.forEach((item) => {
                const key = keyGetter(item);
                const collection = map.get(key);
                if (!collection) {
                    map.set(key, [item]);
                } else {
                    collection.push(item);
                }
            });
            return map;
        },
        increaseSelectedResponseIndex() {
            if (this.response.totalCount === 0) {
                return;
            }
            this.selectedResponseIndex = parseInt(this.selectedResponseIndex);
            if (this.selectedResponseIndex >= this.response.totalCount) {
                this.selectedResponseIndex = this.response.totalCount;
                return;
            }
            this.selectedResponseIndex += 1
        },
        decreaseSelectedResponseIndex() {
            if (this.response.totalCount === 0) {
                return;
            }
            this.selectedResponseIndex = parseInt(this.selectedResponseIndex);
            if (this.selectedResponseIndex <= 1) {
                this.selectedResponseIndex = 1;
                return;
            }
            this.selectedResponseIndex -= 1;
        },
        navigateToIndividualResponse(responseId) {
            const response = this.response.items.find(q => q.id === responseId);
            if (!!response) {
                this.selectedResponse = response;
                this.selectedResponseIndex = this.response.items.findIndex(q => q.id === responseId) + 1;
                this.$refs.tab_header.querySelector("a.active").classList.remove("active");
                this.$refs.tab_body.querySelector("div.active").classList.remove("show", "active");
                $(this.$el.querySelector("#nav-individual-tab")).tab("show");
            }

        }
    },
    watch: {
        'selectedResponseIndex': {
            async handler(newVal) {                
                if (this.response.totalCount === 0) {
                    this.selectedResponseIndex = 0;
                    return;
                }
                
                await this.setResponse(newVal);
            },
            deep: true
        }
    },
    computed: {
        getQuestionsForSummary(){
            return this.questions.filter(q => q.questionType !== 1 && q.questionType !== 2);
        }
    }
})

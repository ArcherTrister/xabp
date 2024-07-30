const vueQuestionComponent = new Vue({
    el: '#vue-questions',
    data: {
        l: abp.localization.getResource('Forms'),
        formAppService: x.forms.forms.form,
        editModal: new abp.ModalManager(abp.appPath + 'Forms/Questions/EditSettingsModal'),
        isLoading: true,
        form: {
            id: '', title: '', description: '', update: function update() {
                return vueQuestionComponent.formAppService.update(this.id, {
                    title: this.title.trim(),
                    description: this.description != null ? this.description.trim() : this.description
                });
            },
            settings: {
                isAcceptingResponses: Boolean
            }
        },
        questions: []
    },
    created() {
    },
    async mounted() {
        this.otherChoice = this.$refs.other.getAttribute('data-value');
        this.form.id = this.$refs.form.getAttribute('value');
        this.form.settings.isAcceptingResponses = Boolean(this.$refs.form.getAttribute('data-settings'));
        await this.getForm(this.form.id);
        this.isLoading = false;
    },
    methods: {
        async getForm(formId) {
            try {
                const result = await this.formAppService.get(formId);
                this.form.id = result.id;
                this.form.title = result.title;
                this.form.description = result.description;
                this.questions = result.questions.map(obj => ({...obj, isDirty: false}));
                this.questions = _.orderBy(this.questions, 'index');
            } catch (error) {
                setTimeout(function () {
                    abp.message.error(error.message);
                }, 500);
            }
        },
        updateForm(submittedForm) {
            this.form.update()
                .then((response) => {
                    abp.notify.success(vueQuestionComponent.l('Saved'));
                })
                .catch(error => {
                    setTimeout(function () {
                        abp.message.error(error.message);
                    }, 500);
                });
        },
        debouncedFormUpdate: _.debounce(function () {
            if (!this.validateFormTitle()) {
                return;
            }
            this.updateForm();
        }, 2000),
        validateFormTitle() {
            return this.form.title.length > 0;
        },
        addQuestion() {
            let item = {
                index: this.questions.length + 1,
                title: this.l("Form:Questions:QuestionIndex", this.questions.length + 1),
                questionType: 1,
                formId: this.form.id,
                choices: []
            };

            this.formAppService.createQuestion(this.form.id, item)
                .then((response) => {
                    let i = this.questions.push(response);
                    this.$nextTick(() => {
                        this.$refs[`questions${i - 1}`][0].$el.querySelector('input[name="Title"]').focus();
                        this.$refs[`questions${i - 1}`][0].$el.querySelector('input[name="Title"]').select();
                    });
                })
                .catch(error => {
                    setTimeout(function () {
                        abp.message.error(error.message);
                    }, 500);
                });
        },
        questionDeleted(deletedQuestionId) {
            this.questions = this.questions.filter(q => q.id !== deletedQuestionId);
        },
        preview() {
            const link = `${location.origin}/Forms/${this.form.id}/Preview`;
            window.open(link, '_blank');
        },
        openSettingsModal() {
            this.editModal.open({id: this.form.id});
        }
    },
    computed: {}
})
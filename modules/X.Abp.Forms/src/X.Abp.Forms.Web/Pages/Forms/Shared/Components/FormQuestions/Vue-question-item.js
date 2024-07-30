Vue.component('question', {
    template: `
    <div v-bind:tabindex="value.index">
        <div class="card question" :id="value.id">
            <div class="card-header">
                <div class="row">
                    <div class="col">
                        <h2 class="card-title">{{l("Form:Questions:QuestionIndex", value.index)}}</h2>
                        <small v-if="isSaving" v-bind:class="{ 'fade-out': isSaving }">Saving...</small>
                        <small v-if="!isSaving" v-bind:class="{ 'fade-in': isSaved }">{{l("Form:Questions:ModifiedAt", lastModificationDateString)}}</small>
                    </div>
                    
                    <div class="col-auto">
                        <div class="form-check form-switch" style="padding-top: 0.5em">
                          <input v-model="value.isRequired" v-on:change="debouncedQuestionUpdate" type="checkbox" class="form-check-input" v-bind:id="'switch_'+value.id">
                          <label class="form-check-label" v-bind:for="'switch_'+value.id">{{l("Form:Questions:Required")}}</label>
                        </div>
                    </div>
                    
                    <div class="col-auto">
                        <button type="submit" class="btn" data-toggle="tooltip" data-placement="bottom" title="" :data-original-title='l("Form:Questions:Save")'>
                            <i class="fa fa-save" aria-hidden="true"></i>
                        </button> 
                    </div>
                    <div class="col-auto">
                        <button v-on:click="deleteQuestion(value.id)" type="button" class="btn" data-toggle="tooltip" data-placement="bottom" title="" :data-original-title='l("Form:Questions:Remove")'>
                            <i class="fa fa-trash" aria-hidden="true"></i>
                        </button>    
                    </div>
                </div>
            </div>
            <div class="card-body">
                <input type="hidden" :value="value.id"/>
                <form v-on:submit.prevent="update" v-on:input="debouncedQuestionUpdate">
                    <div class="container">
                        <div class="row">
                            <div class="col">
                                <div class="form-group mb-3">
                                    <input class="items-card-title form-control " placeholder="Untitled Question"
                                          type="text" name="Title"
                                           v-model="value.title">
                                   <span class="text-danger field-validation-valid" data-valmsg-for="Title" data-valmsg-replace="true"></span>
                                </div>
                            </div>
                            <div class="col">
                                <div class="form-group mb-3">
                                    <question-type :question-type-selected="value.questionType" v-on:changed:question_type="questionTypeChanged"></question-type>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col">
                                <div class="form-group mb-3">
                                    <input class="items-card-description form-control" placeholder="Description" type="text" name="Description"
                                    v-model="value.description">
                                </div>
                            </div>
                        </div>    
                        <choice :question-choices="value.choices" 
                            :question-type="value.questionType" 
                            :has-other="value.hasOtherOption" 
                            v-on:changed:has_other_option="changedHasOtherOption" 
                            v-on:changed:choice="changedChoice"
                            v-if="value.choices != undefined && value.choices.length>0"
                        >                        
                        </choice>                        
                    </div>
                </form>
            </div>
        </div>
    </div>
`,
    data() {
        return {
            questionAdminAppService: x.forms.questions.question,
            l: abp.localization.getResource('Forms'),
            lastModificationDateString: String,
            isSaving: false,
            isSaved: false,
            otherChoiceString: String
        }
    },
    props: ['value'],
    components: {},
    created() {
        this.otherChoiceString = vueQuestionComponent.otherChoice;
    },
    mounted() {
        const creationDate = new Date(this.value.creationTime);
        const updateDate = new Date(this.value.lastModificationTime);
        if (creationDate.getTime() > updateDate.getTime()) {
            this.lastModificationDateString = creationDate.toLocaleString();
        } else {
            this.lastModificationDateString = updateDate.toLocaleString();
        }

    },
    methods: {
        update(submittedForm) {
            this.isSaving = true;
            this.questionAdminAppService.update(this.value.id, this.value)
                .then((response) => {
                    // abp.notify.success(this.l('Saved'));
                    this.lastModificationDateString = new Date(response.lastModificationTime).toLocaleString();
                    this.isSaved = true;
                    setTimeout(() => {
                        this.isSaving = false;
                    }, 500);
                })
                .catch(error => {
                    this.isSaving = false;
                    setTimeout(function () {
                        abp.message.error(error.message);
                    }, 500);
                });
        },
        debouncedQuestionUpdate: _.debounce(function () {
            this.update();
        }, 500),
        deleteQuestion(id) {
            this.questionAdminAppService.delete(id)
                .then((response) => {
                    this.$emit('question_deleted', id);
                    // destroy the vue listeners, etc
                    this.$destroy();
                    // remove the element from the DOM
                    let closestQuestion = this.$el.parentNode.querySelectorAll(".question:not(.selected-question)")[0];
                    if (closestQuestion !== undefined) {
                        $(closestQuestion).trigger("click");
                    }
                    this.$el.parentNode.removeChild(this.$el);
                    abp.notify.success(this.l('Deleted'));
                })
                .catch(error => {
                    setTimeout(function () {
                        abp.message.error(error.message);
                    }, 500);
                });
        },
        questionTypeChanged(oldValue, newValue) {
            const questionsWithoutOtherOption = [1, 2, 5];
            this.$set(this.value, 'questionType', parseInt(newValue));
            if (questionsWithoutOtherOption.includes(newValue)) {
                this.$set(this.value, 'hasOtherOption', false);
            }
            if (isChoosable(newValue)) {
                if (this.value.choices.length === 0) {
                    this.value.choices.push({
                        value: this.l("Choice:Option", 1)
                    })
                }
                // remove hasOther option from dropdown
                if (newValue === 5) {
                    this.value.choices = this.value.choices.filter(el => el.value !== this.otherChoiceString);
                }
            } else {
                //remove choices of none-isChoosable question
                this.value.choices = [];
            }
            this.debouncedQuestionUpdate();
        },
        changedHasOtherOption(newValue) {
            this.$set(this.value, 'hasOtherOption', newValue);
            this.debouncedQuestionUpdate();
        },
        changedChoice(newValue) {
            this.debouncedQuestionUpdate();
        }
    }
});

function isChoosable(value) {
    if (value === 3 || value === 4 || value === 5) {
        return true;
    }
}
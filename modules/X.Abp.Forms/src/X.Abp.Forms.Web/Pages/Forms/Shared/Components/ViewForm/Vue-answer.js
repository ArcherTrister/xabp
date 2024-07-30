// Register the component globally.
Vue.component('ValidationProvider', VeeValidate.ValidationProvider);
Vue.component('ValidationObserver', VeeValidate.ValidationObserver);
//Short-text component
(function () {
    VeeValidate.extend('shortTextValidate', {
        params: ['isRequired'],
        validate: (value, args) => {
            if (!args.isRequired) {
                return true;
            }
            return Boolean(value.trim());
        },
        computesRequired: true
    });

    Vue.component('short-text', {
        template: `
        <div>
            <div class="form-group mb-3 ">
                <label v-bind:for="question.id" class="question-label">
                    {{question.title}} <span v-if="question.isRequired" style="color: red">*</span>
                </label>
                <small class="form-text text-muted">{{question.description}}</small>
            </div>
            <validation-observer :ref="'observer_'+question.id" v-slot="{ invalid }">
                <validation-provider :rules="{ shortTextValidate: { isRequired } }" v-slot="{ valid, errors }">
                    <input type="text" v-bind:id="question.id"
                        v-model="answer.value"
                        v-bind:class="{ 'is-invalid': errors.length>0 }"
                        v-on:input="updateAnswer" 
                         aria-describedby="desc" v-bind:placeholder='l("Answer:AnswerPlaceholder")' class="form-control" >
                    <div v-if="errors.length>0" class="invalid-feedback">{{l("Answer:RequiredQuestion")}}</div>
                </validation-provider>                                 
            </validation-observer>  
        </div>
    `,
        data() {
            return {
                answer: {
                    questionId: "",
                    value: "",
                },
                isRequired: Boolean,
                l: abp.localization.getResource('Forms')
            };
        },
        props: {
            question: {},
            value: {}
        },
        created() {
            this.answer.questionId = this.question.id;
            this.isRequired = this.question.isRequired;
            this.$nextTick(() => {
                this.answer.value = this.value;
            });
        },
        methods: {
            async updateAnswer($event) {
                const observerName = "observer_" + this.question.id;
                const observer = this.$refs[observerName];
                const validationResult = await observer.validateWithInfo();

                if (!validationResult.isValid || !validationResult.flags.dirty) {
                    return;
                }
                this.$emit('answered:short_text', {answer: this.answer});
            },
        }
    });
})();


VeeValidate.extend('checkboxValidate', {
    params: ['currentAnswer', 'required'],
    validate: (value, param) => {
        return !(param.currentAnswer.selectedAnswers.length === 0 && param.required && !param.currentAnswer.value);
    },
    computesRequired: true,
    message: abp.localization.getResource('Forms')("Answer:RequiredQuestion")
});
VeeValidate.extend('checkboxOtherValidate', {
    params: ['value', 'choice', 'currentAnswer'],
    validate: (value, param) => {
        let isOtherField = param.value.hasOtherOption === true && param.choice.value === 'Other...\n';
        let isOtherCheckboxSelected = param.currentAnswer.selectedAnswers.find(o => o.value === 'Other...\n') !== undefined;
        if (isOtherField && isOtherCheckboxSelected) {
            return !(!param.currentAnswer.value);
        }
    },
    computesRequired: true,
    message: abp.localization.getResource('Forms')("Answer:RequiredField")
});

VeeValidate.extend('choiceMultipleValidate', {
    params: ['currentAnswer', 'required'],
    validate: (value, param) => {
        return !(param.required && !param.currentAnswer.value);
    },
    computesRequired: true,
    message: abp.localization.getResource('Forms')("Answer:RequiredQuestion")
});
VeeValidate.extend('choiceMultipleOtherValidate', {
    params: ['value', 'choice', 'currentAnswer', 'otherValue'],
    validate: (value, param) => {
        let isOtherField = param.value.hasOtherOption === true && param.choice.value === 'Other...\n';
        let isOtherMultiChoiceSelected = param.currentAnswer.value === 'Other...\n';
        if (isOtherField && isOtherMultiChoiceSelected) {
            return !(!param.otherValue);
        }
    },
    computesRequired: true,
    message: abp.localization.getResource('Forms')("Answer:RequiredField")
});
VeeValidate.extend('dropdownValidate', {
    params: ['currentAnswer', 'required'],
    validate: (value, param) => {
        return !(param.required && !param.currentAnswer.value);
    },
    computesRequired: true,
    message: abp.localization.getResource('Forms')("Answer:RequiredQuestion")
});

Vue.component('answer', {
    template: `
    <div>
        <div class="card" >
            <div class="card-body">
                <div class="container">                
                    <!-- SHORT TEXT BEGINS -->
                    <div v-if="value.questionType === 1">                        
                        <short-text :question="value" :value="currentAnswer.value" v-on:answered:short_text="saveText"></short-text>               
                    </div>
                    <!-- SHORT TEXT ENDS -->
                    
                    <!-- CHECKBOX BEGINS -->
                    <div v-if="value.questionType === 4" v-on:input="debouncedCheckboxUpdate">
                        <div class="form-group mb-3 ">
                            <label v-bind:for="value.id" class="question-label">
                                {{value.title}} <span v-if="value.isRequired" style="color: red">*</span>
                            </label>
                            <small class="form-text text-muted">{{value.description}}</small>
                        </div>
                        <validation-observer :ref="'observer_'+value.id" v-slot="{ invalid }">
                            <validation-provider :rules="{ checkboxValidate: {currentAnswer,required} }" v-slot="{ validate, valid, errors }">
                                <div class="choice mb-1" v-for="(choice, index) in sortedChoices">                        
                                    <div class="form-check">
                                        <input type="checkbox" v-bind:id="choice.id" tabindex="-1"
                                            ref="cbSelected"   
                                            v-bind:value="choice"                            
                                            v-model="currentAnswer.selectedAnswers" 
                                            v-on:change="debouncedCheckboxUpdate"
                                            v-bind:class="{ 'is-invalid':  errors.length>0 }" 
                                            aria-describedby="desc" class="form-check-input">  
                                        <label class="form-check-label" v-bind:for="choice.id">                                        
                                            <div v-if="isOtherField(choice) && isOtherCheckboxSelected()" style="margin-top:-.5rem; display: flex; gap: 1rem;">
                                                <span>Other:</span>           
                                                <validation-provider :rules="{ checkboxOtherValidate:{value, choice, currentAnswer} }" v-slot="{ validate, valid, errors }">
                                                    <input type="text" v-bind:id="choice.id"
                                                        v-model="currentAnswer.value"
                                                        v-on:blur="validate"
                                                        v-bind:class="{ 'is-invalid': errors.length>0 }"
                                                        v-bind:required="value.isRequired" aria-describedby="desc" v-bind:placeholder='l("Answer:AnswerPlaceholder")' class="form-control"
                                                        style="min-width: 400px">
                                                    <div class="invalid-feedback">{{l("Answer:RequiredField")}}</div>
                                                </validation-provider>    
                                            </div>                                        
                                            <div v-else>{{choice.value}}</div>
                                        </label>
                                    </div>                   
                                </div>
                                <div v-if="errors.length>0" ref="cbErr" v-bind:class="{ 'is-invalid': errors.length>0 }" style="width: 100%; margin-top: 0.25rem; font-size: 80%;color: #dc3545;">{{l("Answer:RequiredQuestion")}}</div>

                            </validation-provider>
                        </validation-observer>
                    </div>    
                    <!-- CHECKBOX ENDS -->
                    
                    <!-- CHOICE MULTIPLE BEGINS -->
                    <div v-if="value.questionType === 3">
                        <div class="form-group mb-3 ">
                            <label v-bind:for="value.id" class="question-label">
                                {{value.title}} <span v-if="value.isRequired" style="color: red">*</span>
                            </label>
                            <small class="form-text text-muted">{{value.description}}</small>
                        </div>
                        <validation-observer :ref="'observer_'+value.id" v-slot="{ invalid }">
                        <validation-provider :rules="{ choiceMultipleValidate: {currentAnswer,required} }" v-slot="{ validate, valid, errors }">
                            <div class="" v-for="(choice, index) in sortedChoices">
                                <div class="form-check">                          
                                    <input type="radio" v-bind:id="choice.id" v-bind:name="value.id" 
                                        v-bind:value="choice.value"                            
                                        v-model="currentAnswer.value" 
                                        v-on:change="validate" 
                                        v-on:input="saveChoiceMultiple($event, choice.id, value.id, currentAnswer)" 
                                        v-bind:class="{ 'is-invalid': errors.length }" 
                                        v-bind:required="value.isRequired" aria-describedby="desc" class="form-check-input" >       
                                            
                                    <label class="form-check-label" v-bind:for="choice.id">
                                        <div v-if="isOtherField(choice) && currentAnswer.value==='Other...\\n'" style="margin-top:-.5rem; display: flex; gap: 1rem">
                                            <span>Other:</span>        
                                            <validation-provider :rules="{ choiceMultipleOtherValidate:{value, choice, currentAnswer, otherValue} }" v-slot="{ validate, valid, errors }">
                                                <input type="text" v-bind:id="choice.id"
                                                    v-model="otherValue"
                                                    v-on:blur="validate"
                                                    v-on:input="saveChoiceMultiple($event, choice.id, value.id, currentAnswer)"
                                                    v-bind:class="{ 'is-invalid': errors.length }"
                                                    v-bind:required="value.isRequired" aria-describedby="desc" v-bind:placeholder='l("Answer:AnswerPlaceholder")' class="form-control"
                                                    style="min-width: 400px">
                                                <div class="invalid-feedback">{{l("Answer:RequiredField")}}</div>
                                            </validation-provider>    
                                        </div>
                                        <div v-else>{{choice.value}}</div>                                   
                                    </label>                 
                                </div>                            
                            </div>
                            <div v-if="errors.length>0" v-bind:class="{ 'is-invalid': errors.length>0 }" style="width: 100%; margin-top: 0.25rem; font-size: 80%;color: #dc3545;">{{l("Answer:RequiredQuestion")}}</div>
                        </validation-provider>
                        </validation-observer>
                    </div>    
                    <!-- CHOICE MULTIPLE ENDS -->
                    
                    <!-- DROPDOWN BEGINS -->
                    <div v-if="value.questionType === 5">
                        <div class="form-group mb-3 ">
                            <label v-bind:for="value.id" class="question-label">
                                {{value.title}} <span v-if="value.isRequired" style="color: #ff0000">*</span>
                            </label>
                            <small class="form-text text-muted">{{value.description}}</small>
                        </div>
                        <validation-observer :ref="'observer_'+value.id" v-slot="{ invalid }">
                            <validation-provider :rules="{ dropdownValidate: {currentAnswer,required} }" v-slot="{ validate, valid, errors }">
                                <select class="form-select" 
                                    v-on:change="saveDropDown($event)"  
                                    v-bind:class="{ 'is-invalid': errors.length>0 }" 
                                    v-model="currentAnswer.value"
                                    v-bind:required="value.isRequired" >
                                        <option v-for="(choice, index) in sortedChoices" v-bind:value="choice.value" v-bind:id="choice.id" class="option-size-initial">{{choice.value}}</option>
                                </select>
                                <div class="invalid-feedback">{{l("Answer:RequiredQuestion")}}</div>
                            </validation-provider>                      
                        </validation-observer>
                    </div>    
                    <!-- DROPDOWN ENDS -->
                </div>
            </div>
        </div>
    </div>
`,
    data() {
        return {
            answers: [],
            currentAnswer: {
                questionId: "",
                choiceId: "",
                value: "",
                hasError: false,
                selectedAnswers: []
            },
            required: Boolean,
            postList: [],
            otherValue: "",
            l: abp.localization.getResource('Forms')
        };
    },
    props: ['value', 'isEditing'],
    created() {
    },
    mounted() {
        this.required = this.value.isRequired;
        this.initAnswers();
    },
    computed: {
        sortedChoices: function () {
            return _.orderBy(this.value.choices, 'index')
        }
    },
    methods: {
        saveText($event) {
            this.currentAnswer.value = $event.answer.value;
            this.postList = [];
            this.postList.push({
                questionId: $event.answer.questionId,
                value: $event.answer.value
            });
            this.broadcastAnswer();
            this.debouncedTextUpdate();
        },
        debouncedTextUpdate: _.debounce(function () {
            //save this.postList
            console.log(`Saving Text ${this.postList[0].value}`);
        }, 250),
        async saveDropDown($event) {
            const observerName = "observer_" + this.value.id;
            const observer = this.$refs[observerName];
            const validationResult = await observer.validateWithInfo();
            if (validationResult.errors.length > 0) {
                return;
            }

            this.postList = [];
            this.postList.push({
                questionId: this.value.id,
                value: this.currentAnswer.value,
                choiceId: $event.target.querySelector('option:checked').id
            });
            this.broadcastAnswer();
            this.debouncedDropdownUpdate();
        },
        debouncedDropdownUpdate: _.debounce(function () {
            //save this.postList
        }, 250),
        saveChoiceMultiple: _.debounce(function (e, choiceId, questionId) {
            this.currentAnswer.choiceId = choiceId;
            this.currentAnswer.questionId = questionId;
            const observerName = "observer_" + this.value.id;
            const observer = this.$refs[observerName];
            observer.validate()
                .then(res => {
                        if (!res) {
                            return;
                        }
                        this.postList = [];

                        let obj = {
                            questionId: this.value.id,
                            choiceId: this.currentAnswer.choiceId,
                            value: this.currentAnswer.value
                        };
                        if (this.currentAnswer.value === 'Other...\n') {
                            obj.value = this.otherValue;
                        }

                        this.postList.push(obj);
                        //save
                        this.broadcastAnswer();
                        console.table(`Saving Choice Multiple ${JSON.stringify(this.postList)}`);
                    }
                )
        }, 250),
        debouncedCheckboxUpdate: _.debounce(function () {
            const observerName = "observer_" + this.value.id;
            const observer = this.$refs[observerName];
            observer.validate()
                .then(res => {
                    if (!res) {
                        return;
                    }
                    this.postList = [];
                    this.currentAnswer.selectedAnswers.forEach((v, i) => {
                        let obj = {
                            questionId: this.value.id,
                            choiceId: v.id,
                            value: v.value
                        };
                        if (v.value === 'Other...\n') {
                            obj.value = this.currentAnswer.value;
                        }

                        this.postList.push(obj);
                        this.broadcastAnswer();
                        console.log(`Saving Checkboxes ${JSON.stringify(this.postList)}`);
                    });
                    // removed checkbox
                    if (this.currentAnswer.selectedAnswers.length === 0) {
                        this.broadcastAnswer();
                    }
                });
        }, 250),
        isOtherField(choice) {
            return this.value.hasOtherOption === true && choice.value === 'Other...\n';
        },
        // validate() {
        //     if (this.value.isRequired && !this.currentAnswer.value) {
        //         this.currentAnswer.hasError = true;
        //         return false;
        //     } else {
        //         this.currentAnswer.hasError = false;
        //         return true;
        //     }
        // },
        validateOtherField() {
            if (this.value.isRequired && !this.otherValue) {
                this.currentAnswer.hasError = true;
                return false;
            } else {
                this.currentAnswer.hasError = false;
                return true;
            }
        },
        validateCheckbox() {
            return !(this.currentAnswer.selectedAnswers.length === 0 && this.value.isRequired);
        },
        isOtherCheckboxSelected() {
            return this.currentAnswer.selectedAnswers.find(o => o.value === 'Other...\n') !== undefined;
        },
        isOtherMultiChoiceSelected() {
            return this.currentAnswer.value === 'Other...\n';
        },
        broadcastAnswer() {
            //broadcast and submit
            this.$emit('updated:answer', {questionId: this.value.id, answers: this.postList});
        },
        initAnswers() {
            this.answers = [];
            if (this.value.answers !== undefined && this.value.answers.length > 0) {
                this.answers = this.value.answers.filter(item => {
                    return item.questionId === this.value.id
                });
            } else if (JSON.parse(localStorage.getItem(this.value.formId)) !== null) {
                this.answers = JSON.parse(localStorage.getItem(this.value.formId)).filter(e => {
                    return e.questionId === this.value.id
                });
            } else {
                return;
            }
            this.currentAnswer.questionId = this.value.id;
            if ((this.value.questionType === 1 || this.value.questionType === 2) && this.answers.length > 0) { //Short text or Paragraph                    
                this.currentAnswer.value = this.answers[0].value;
                if (this.isEditing) {
                    let answers = JSON.parse(localStorage.getItem(this.value.formId)) || [];
                    if (answers.length === 0) {
                        answers.push({
                            questionId: this.currentAnswer.questionId,
                            value: this.currentAnswer.value
                        });
                        localStorage.setItem(this.value.formId, JSON.stringify(answers));
                    }
                }
            }
            if (this.value.questionType === 3 && this.answers.length > 0) { //Choice Multiple
                const selectedChoice = this.value.choices.find(e => e.id === this.answers[0].choiceId);
                this.currentAnswer.value = selectedChoice.value;
                this.currentAnswer.choiceId = selectedChoice.id;
                if (this.isOtherMultiChoiceSelected()) {
                    this.otherValue = this.answers[0].value;
                }
                if (this.isEditing) {
                    let answers = JSON.parse(localStorage.getItem(this.value.formId)) || [];
                    const question = answers.find(q => q.questionId === this.value.id);
                    if (!question) {
                        let obj = {
                            questionId: this.value.id,
                            choiceId: this.currentAnswer.choiceId,
                            value: this.currentAnswer.value
                        };
                        if (this.currentAnswer.value === 'Other...\n') {
                            obj.value = this.otherValue;
                        }
                        answers.push(obj);
                        localStorage.setItem(this.value.formId, JSON.stringify(answers));
                    }
                }
            }
            if (this.value.questionType === 4 && this.answers.length > 0) { //Checkbox
                this.answers.forEach(item => {
                    const choice = this.value.choices.find(t => t.id === item.choiceId);
                    this.currentAnswer.selectedAnswers.push(choice);
                    if (choice.value === 'Other...\n') {
                        this.currentAnswer.choiceId = choice.id;
                        this.currentAnswer.value = item.value;
                    }
                    if (this.isEditing) {
                        let answers = JSON.parse(localStorage.getItem(this.value.formId)) || [];
                        this.currentAnswer.selectedAnswers.forEach((v, i) => {
                            const question = answers.find(q => q.questionId === this.value.id && q.choiceId === v.id);
                            if (!question) {
                                let obj = {
                                    questionId: this.value.id,
                                    choiceId: v.id,
                                    value: v.value
                                };
                                if (v.value === 'Other...\n') {
                                    obj.value = this.currentAnswer.value;
                                }
                                answers.push(obj);
                            }
                        });
                        localStorage.setItem(this.value.formId, JSON.stringify(answers));
                    }
                })
            }
            if (this.value.questionType === 5 && this.answers.length > 0) { //Dropdown
                const selectedChoice = this.value.choices.find(e => e.id === this.answers[0].choiceId)
                this.currentAnswer.value = selectedChoice.value;
                if (this.isEditing) {
                    let answers = JSON.parse(localStorage.getItem(this.value.formId)) || [];
                    const question = answers.find(q => q.questionId === this.value.id);
                    if (!question) {
                        answers.push({
                            questionId: this.value.id,
                            value: this.currentAnswer.value,
                            choiceId: selectedChoice.id
                        });
                        localStorage.setItem(this.value.formId, JSON.stringify(answers));
                    }
                }
            }
        }
    }
})



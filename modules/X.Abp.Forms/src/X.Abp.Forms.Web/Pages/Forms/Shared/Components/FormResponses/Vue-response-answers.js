Vue.component('response-answer', {
    template: `
    <div>
        <div class="card" >
            <div class="card-body">
                <div class="container">
                        <!-- short-text -->
                        <div v-if="value.questionType===1">
                            <div class="form-group mb-3 ">
                                <label v-bind:for="value.id" >
                                    {{value.title}} <span v-if="value.isRequired" style="color: red">*</span>
                                </label>
                                <small class="form-text text-muted">{{value.description}}</small>
                            </div>
                            <label class="form-control">{{currentAnswer.value}} </label>
                        </div>    
                        <!-- short-text ends -->
                        
                        <!-- choice-multiple -->  
                        <div v-if="value.questionType===3">                            
                            <div class="form-group mb-3 ">
                            <label v-bind:for="value.id" >
                                {{value.title}} <span v-if="value.isRequired" style="color: red">*</span>
                            </label>
                            <small class="form-text text-muted">{{value.description}}</small>
                            </div>                            
                            <div class="" v-for="(choice, index) in orderedChoices">
                                <div class="form-check">                          
                                    <input disabled type="radio":id="choice.id" 
                                        v-bind:value="choice.value"                            
                                        v-model="currentAnswer.value" 
                                        class="form-check-input" >       
                                            
                                    <label class="form-check-label" v-bind:for="choice.id">
                                        <div v-if="isOtherField(choice) && currentAnswer.value===otherChoice" style="display: flex; gap: 1rem;">
                                            <span>Other:</span>         
                                            <input type="text" disabled v-bind:id="choice.id" 
                                                v-model="otherValue"
                                                class="form-control"
                                                style="min-width: 400px">
                                        </div>
                                        <div v-else>{{choice.value}}</div>                                   
                                    </label>                 
                                </div>                            
                        </div>
                        </div>
                        <!-- choice-multiple ends -->
                        
                        <!-- checkbox -->  
                        <div v-if="value.questionType===4">                            
                            <div class="form-group mb-3 ">
                            <label v-bind:for="value.id" >
                                {{value.title}} <span v-if="value.isRequired" style="color: red">*</span>
                            </label>
                            <small class="form-text text-muted">{{value.description}}</small>
                            </div>
                            <div class="choice mb-1" v-for="(choice, index) in orderedChoices">
                                <div class="form-check">
                                    <input disabled type="checkbox" v-bind:id="choice.id" tabindex="-1"
                                        v-bind:value="choice"
                                        v-model="currentAnswer.selectedAnswers"
                                        aria-describedby="desc" class="form-check-input">
                                    <label class="form-check-label" v-bind:for="choice.id">
                                        <div v-if="isOtherField(choice) && isOtherCheckboxSelected()" style="margin-top:-.5rem; display: flex; gap: 1rem">
                                            <span>Other:</span>                                                                     
                                                <input disabled type="text" v-bind:id="choice.id"                    
                                                    v-model="currentAnswer.value"
                                                    class="form-control"
                                                    style="min-width: 400px">
                                            </div>
                                        <div v-else>{{choice.value}}</div>
                                    </label>
                                </div>                   
                            </div>
                        </div>
                        <!-- checkbox ends --> 
                        
                        <!-- dropdown -->
                        <div v-if="value.questionType === 5">
                        <div class="form-group mb-3">
                            <label v-bind:for="value.id" >
                                {{value.title}} <span v-if="value.isRequired" style="color: #ff0000">*</span>
                            </label>
                            <small class="form-text text-muted">{{value.description}}</small>
                            </div>
                            <select disabled class="form-select" v-model="currentAnswer.value">
                                <option v-for="(choice, index) in orderedChoices" v-bind:value="choice.value" v-bind:id="choice.id" class="option-size-initial">{{choice.value}}</option>
                            </select>
                        </div> 
                        <!-- dropdown ends -->
                        
                    <div>
                </div>
            </div>
        </div>
    </div>
</div>
`,
    data() {
        return {
            answers: [],
            orderedChoices: [],
            currentAnswer: {
                questionId: "",
                choiceId: "",
                value: "",
                hasError: false,
                selectedAnswers: []
            },
            otherChoice: String,
            otherValue: "",
            l: abp.localization.getResource('Forms')
        };
    },
    props: {
        'value': {type: Object},
        'response': {
            type: Object, default() {
                return {
                    answers: []
                }
            }
        }
    },
    created() {
        this.otherChoice = vueResponseComponent.otherChoice;
    },
    async mounted() {
        // await this.$nextTick();
        this.initAnswers();
        this.orderedChoices = _.orderBy(this.value.choices, 'index');
    },
    methods: {
        isOtherMultiChoiceSelected() {
            return this.currentAnswer.value === this.otherChoice;
        },
        isOtherCheckboxSelected() {
            return this.currentAnswer.selectedAnswers.find(o => o.value === this.otherChoice) !== undefined;
        },
        isOtherField(choice) {
            return this.value.hasOtherOption === true && choice.value === this.otherChoice;
        },
        initAnswers() {
            this.answers = this.response.answers.filter(q => q.questionId === this.value.id);
            //Question left blank
            if (this.answers.length === 0) {
                this.currentAnswer = {
                    questionId: "",
                    choiceId: "",
                    value: "",
                    hasError: false,
                    selectedAnswers: []
                };
                this.otherValue = "";
            }
            this.currentAnswer.questionId = this.value.id;
            if ((this.value.questionType === 1 || this.value.questionType === 2) && this.answers.length > 0) { //Short text or Paragraph                    
                this.currentAnswer.value = this.answers[0].value;
            }
            if (this.value.questionType === 3 && this.answers.length > 0) { //Choice Multiple
                const selectedChoice = this.value.choices.find(e => e.id === this.answers[0].choiceId);
                if (selectedChoice === undefined) {
                    //Todo: Question left blank
                } else {
                    this.currentAnswer.value = selectedChoice.value;
                    this.currentAnswer.choiceId = selectedChoice.id;
                }

                if (this.isOtherMultiChoiceSelected()) {
                    this.otherValue = this.answers[0].value;
                }
            }
            if (this.value.questionType === 4 && this.answers.length > 0) { //Checkbox
                this.currentAnswer.selectedAnswers = [];
                this.answers.forEach(item => {
                    const choice = this.value.choices.find(t => t.id === item.choiceId);
                    this.currentAnswer.choiceId = choice.id;
                    if (this.isOtherField(choice)) {
                        this.currentAnswer.value = item.value;
                    }
                    this.currentAnswer.selectedAnswers.push(choice);
                })
            }
            if (this.value.questionType === 5 && this.answers.length > 0) { //Dropdown
                this.currentAnswer.value = this.value.choices.find(e => e.id === this.answers[0].choiceId).value;
            }
        }
    },
    watch: {
        'response': {
            handler(data, oldData) {
                if (this.response.totalCount === 0) {
                    return;
                }
                this.answers = this.response.answers;
                this.initAnswers();
            },
            deep: true
        }
    }
})
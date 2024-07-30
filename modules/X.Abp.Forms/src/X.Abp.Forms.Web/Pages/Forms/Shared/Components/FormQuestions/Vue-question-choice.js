Vue.component('choice', {
    template: `
        <div>
            <!-- Checkbox -->
            <div v-if="questionType === 4">
                <div class="row" v-for="(choice, index) in questionChoices" v-if="choice.value != otherChoiceString">
                    <div class="col-10">
                        <div class="input-group mb-3">
                            <div class="input-group-text">
                                <input disabled="disabled" type="checkbox">
                            </div>                           
                            <input ref="option" v-model="choice.value" v-on:blur="validateChoice" type="text" class="form-control" :disabled="checkReadOnly(choice)">
                        </div>
                    </div>
                    <div class="col-1 can-be-hidden">
                        <button tabindex="-1" v-if="questionChoices.length>1" v-on:click="remove(index)" type="button" class="btn" data-toggle="tooltip" data-placement="bottom" title="" :data-original-title='l("Choice:Remove")'>
                            <i class="fa fa-times" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>
                <div class="row" v-if="hasOther">                
                    <div class="col-10">
                        <div class="input-group mb-3">
                            <div class="input-group-text">
                                <input disabled="disabled" type="checkbox">
                            </div>
                            <input v-if="otherChoice" ref="option" v-model='l("Choice:Other")' v-on:blur="validateChoice" type="text" class="form-control" :disabled="checkReadOnly(otherChoice)">                            
                        </div>
                    </div>
                    <div class="col-1 can-be-hidden">
                        <button tabindex="-1" v-if="questionChoices.length>1" v-on:click="removeOther" type="button" class="btn" data-toggle="tooltip" data-placement="bottom" title="" :data-original-title='l("Choice:Remove")'>
                            <i class="fa fa-times" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>
                
                <div class="row can-be-hidden"> 
                    <div class="col-8">
                        <div class="input-group mb-3">
                            <div class="input-group-text">
                                <input disabled="disabled" type="checkbox">
                            </div>
                            <div class="add-option">
                                <div>
                                    <a class="text-start" v-on:click="add" href="javascript:void(0)">
                                        <span> {{l("Choice:AddOption")}} </span>
                                    </a> <span v-if="!hasOther">{{l("Choice:Or")}}</span> 
                                    <a v-on:click="addOther" href="javascript:void(0)" v-if="!hasOther"> 
                                    <span> {{l("Choice:AddOther")}} </span> </a>
                                </div>
                            </div>        
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- ChoiceMultiple -->
            <div v-if="questionType === 3">
                <div class="row" v-for="(choice, index) in questionChoices" v-if="choice.value != otherChoiceString">
                    <div class="col-10">
                        <div class="input-group mb-3">
                            <div class="input-group-text">
                                <input disabled="disabled" type="radio">
                            </div>                           
                            <input ref="option" v-model="choice.value" v-on:blur="validateChoice" type="text" class="form-control" :disabled="checkReadOnly(choice)">
                        </div>
                    </div>
                    <div class="col-1 can-be-hidden">
                        <button tabindex="-1" v-if="questionChoices.length>1" v-on:click="remove(index)" type="button" class="btn" data-toggle="tooltip" data-placement="bottom" title="" :data-original-title='l("Choice:Remove")'>
                            <i class="fa fa-times" aria-hidden="true"></i>
                        </button>
                    </div>
                </div v-if="choice.value != otherChoiceString">
                
                <div class="row" v-if="hasOther">                
                    <div class="col-10">
                        <div class="input-group mb-3">
                            <div class="input-group-text">
                                <input disabled="disabled" type="radio">
                            </div>
                            <input v-if="otherChoice" ref="option" v-model='l("Choice:Other")' v-on:blur="validateChoice" type="text" class="form-control" :disabled="checkReadOnly(otherChoice)">                            
                        </div>
                    </div>
                    <div class="col-1 can-be-hidden">
                        <button tabindex="-1" v-if="questionChoices.length>1" v-on:click="removeOther" type="button" class="btn" data-toggle="tooltip" data-placement="bottom" title="" :data-original-title='l("Choice:Remove")'>
                            <i class="fa fa-times" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>
                
                <div class="row can-be-hidden"> 
                    <div class="col-8">
                        <div class="input-group mb-3">
                            <div class="input-group-text">
                                <input disabled="disabled" type="radio">
                            </div>
                            <div class="add-option">
                                <div>
                                    <a class="text-start" v-on:click="add" href="javascript:void(0)">
                                        <span> {{l("Choice:AddOption")}} </span>
                                    </a> <span v-if="!hasOther">{{l("Choice:Or")}}</span> 
                                    <a v-on:click="addOther" href="javascript:void(0)" v-if="!hasOther"> 
                                        <span> {{l("Choice:AddOther")}} </span> 
                                    </a>
                                </div>
                            </div>        
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Dropdown -->
            <div v-if="questionType === 5">
                <div class="row" v-for="(choice, index) in questionChoices">
                    <div class="col-10">
                        <div class="input-group mb-3">
                            <div class="input-group-text">
                                <span>{{index+1}}</span>
                            </div>
                            <input ref="option" v-model="choice.value" type="text" class="form-control">
                        </div>
                    </div>
                    <div class="col-1 can-be-hidden">
                        <button v-if="questionChoices.length>1" v-on:click="remove(index)" type="button" class="btn" data-toggle="tooltip" data-placement="bottom" title="" data-original-title="Remove">
                            <i class="fa fa-times" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>
                <div class="row can-be-hidden"> 
                    <div class="col-8">
                        <div class="input-group mb-3">
                            <div class="input-group-text">
                                <span>{{(questionChoices.length+1)}}</span>
                            </div>
                            <div class="add-option" style="padding-top: 0.5em">
                                <a class="text-start" v-on:click="add" href="javascript:void(0)">
                                    <span> {{l("Choice:AddOption")}} </span>
                                </a>
                            </div>        
                        </div>
                    </div>
                </div>
            </div>
        </div>`,
    data() {
        return {
            l: abp.localization.getResource('Forms'),
            otherChoiceString: String,
            otherChoice: {}
        }
    },
    props: {
        questionType: {
            default: null
        },
        questionChoices: {
            type: Array,
            default() {
                return []
            },
        },
        hasOther: Boolean,
    },
    computed: {},
    created() {
        this.otherChoiceString = vueQuestionComponent.otherChoice;
    },
    mounted() {
        this.otherChoice = this.questionChoices.find(q => q.value === this.otherChoiceString);
    },
    methods: {
        remove(index) {
            const item = this.questionChoices[index];
            
            if (this.checkReadOnly(item)) {
                this.$emit('changed:has_other_option', false);
            }
            this.questionChoices.splice(index, 1);
            this.$emit('changed:choice');
        },
        removeOther(){
            const indexOfOther = this.questionChoices.findIndex(q => q.value === this.otherChoiceString);
            this.remove(indexOfOther);
        }, 
        add() {
            let index = this.questionChoices.length + 1;

            if (this.questionChoices.find(o => o.readOnly)) {
                index = this.questionChoices.length;
            }
            this.questionChoices.push({
                value: this.l("Choice:Option")
            });
            this.otherChoice = this.questionChoices.find(q => q.value === this.otherChoiceString);

            this.$nextTick(() => {
                // this.$refs[`items${i - 1}`][0].$el.querySelector('input[name="Title"]').focus()
                let options = this.$refs.option;
                options[options.length - 1].focus();
                options[options.length - 1].select();
                this.$emit('changed:choice');
            });
        },
        addOther() {
            this.questionChoices.push({
                value: this.otherChoiceString,
                readOnly: true
            });
            this.$emit('changed:has_other_option', true);
            this.otherChoice = this.questionChoices.find(q => q.value === this.otherChoiceString);
        },
        checkReadOnly(choice) {
            return choice.value === this.otherChoiceString;
        },
        validateChoice() {
            this.questionChoices.forEach((v, i) => {
                if (v.value === '') {
                    v.value = this.l("Choice:Option", v.index);
                }
            });
        }
    }
});

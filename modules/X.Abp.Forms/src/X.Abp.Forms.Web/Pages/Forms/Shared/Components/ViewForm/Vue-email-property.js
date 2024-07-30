(function () {
    Vue.component('email-property', {
        template: `
        <div class="card" v-if="display">
            <div class="card-body">
                <div class="container">
                    <div class="form-group mb-3 ">
                        <label class="question-label">{{propertyName}} <span v-if="rules.required" style="color: red">*</span></label>
                        <small class="form-text text-muted" v-cloak>{{input.descriptionText}}</small>
                    </div>
                    <validation-provider :rules="rules" v-slot="{ valid, errors }">
                        <input type="text" name="email"
                               v-model="newValue"
                               v-on:blur="updateValue($event,errors)"
                               v-bind:class="{ 'is-invalid': errors.length>0 }"
                               :required="rules.required"
                               aria-describedby="desc" :placeholder="input.placeholderText" class="form-control">
                        <div v-if="errors.length>0" class="invalid-feedback" v-cloak>{{input.errorText}}</div> 
                    </validation-provider>
                </div>
            </div>
        </div>
    `,
        data() {
            return {
                l: abp.localization.getResource('Forms'),
                newValue: "",
                propertyName:"Email",
                isCollectingEmail: true,
                input: {
                    descriptionText: "",
                    errorText: "",
                    placeholderText: "",
                },
                rules: {
                    required: true,
                    email: true
                }
            };
        },
        props: ["data", "display"],
        mounted() {
            this.newValue = this.data;
        },
        created() {            
            this.init();
        },
        methods: {
            init() {
                this.setInputTexts();
            },
            setInputTexts() {
                this.input.description = this.l("ViewForm:EnterPropertyDesc", this.propertyName);
                this.input.errorText = this.l("ViewForm:InvalidPropertyError", this.propertyName);
                this.input.placeholder = this.l("ViewForm:PropertyPlaceholder", this.propertyName);
            },
            updateValue($event, errors) {
                if (errors.length > 0) {
                    return;
                }
                if (this.newValue === this.data) {
                    return;
                }
                this.$emit('updated:email', {value: this.newValue});
            },
        },
        watch: {
            'data': {
                handler(data, oldData) {
                    this.newValue = data;
                },
                deep: true
            },
            display:{
                handler(data, oldData) {
                    this.display = data;
                },
                deep: true
            }
        }
    });
})();
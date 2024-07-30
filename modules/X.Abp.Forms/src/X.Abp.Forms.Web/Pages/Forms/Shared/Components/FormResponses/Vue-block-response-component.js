Vue.component('block-response', {
    template: `
  	<div>
        <div class="form-group mb-3">
            <label>{{question.title}}</label>
            <small class="form-text text-muted">{{answers.length}} Responses</small>
        </div>
        <div class="block-response-container">
            <div v-for="ans in answers" class="form-group mb-3">  	        
                <label class="form-control">{{ans.value}} </label>
            </div>
        </div>  	    
    </div>
  `,
    props: ['responses', 'question'],
    data() {
        return {
            answers: []
        }
    },
    mounted() {
        this.initialize(this.responses);
    },
    methods: {
        initialize(data) {
            this.answers = [];
            data.forEach(rp => {
                this.answers.push(rp.answers.filter(q => q.questionId === this.question.id));
            });
            this.answers = this.answers.flat();
        }
    },
    watch: {
        'responses': {
            handler(data, oldData) {
                this.initialize(data);
            },
            deep: true
        }
    }

})
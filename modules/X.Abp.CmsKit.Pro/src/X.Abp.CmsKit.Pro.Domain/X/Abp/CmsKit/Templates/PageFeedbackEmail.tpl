<div style="max-width: 600px; margin: 20px auto;">
    <h2>{{model.title}}</h2>
    <p>Entity Type: {{model.entity_type}}</p>
    <p>Url: <a href="{{model.url}}">{{model.url}}</a></p>
    <p>
    Is Useful: {{if model.is_useful }} Yes {{else}} No {{end}} </p>
    <strong>User Note:</strong>
    <div style="background-color: #f9f9f9; padding: 10px; border-radius: 5px;">
        <p>{{model.user_note}}</p>
    </div>
</div>

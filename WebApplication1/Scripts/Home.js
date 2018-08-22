$(document).ready(function ()
{
    hljs.configure({ useBR: true });
    $('pre code').each(function (i, block) {
        hljs.highlightBlock(block);
    });

	var content = $.ajax({ url: "/Home/GetAllEntityName", async: false });

	var jsonObj = eval('(' + content.responseText + ')');
	var entities = jsonObj.Entities;
	var entityArray = [];
	var fieldArray = [];
	var returnSearch;

	for (i = 0; i < entities.length; i++)
	{
		var entity = entities[i];
		entityArray.push(entity.Name)
		var fields = entity.Fields;

		for (y = 0 ; y < fields.length; y++)
		{
			fieldArray.push(fields[y].FieldProperties[0].PropertyValue);
		}

	}

	var substringMatcher = function (strs)
	{
		return function findMatches(q, cb)
		{
			var matches, substrRegex;
			matches = [];
			substrRegex = new RegExp(q, 'i');

			$.each(strs, function (i, str)
			{

				if (substrRegex.test(str))
				{
					matches.push({ value: str });
				}

			});
			cb(matches);
		};
	};


	// Defining the local dataset
	//var nbaTeams = new Bloodhound({
	//	datumTokenizer: Bloodhound.tokenizers.obj.whitespace('team'),
	//	queryTokenizer: Bloodhound.tokenizers.whitespace,
	//	//remote: '/Home/GetAllEntityName?',
	//	prefetch: entities
	//});

	//var nhlTeams = new Bloodhound({
	//	datumTokenizer: Bloodhound.tokenizers.obj.whitespace('team'),
	//	queryTokenizer: Bloodhound.tokenizers.whitespace,
	//	//remote: '/Home/GetAllEntityName?',
	//	prefetch: entities
	//});

	//nbaTeams.initialize();
	//nhlTeams.initialize();

	$('#multiple-datasets .typeahead').typeahead({
		highlight: true
	},
	{
		name: 'Entity',
		display: 'value',
		source: substringMatcher(entityArray),
		templates: {
			header: '<h3 class="league-name">Entity</h3>'
		}
	},
	{
		name: 'Fields',
		display: 'value',
		source: substringMatcher(fieldArray),
		templates: {
			header: '<h3 class="league-name">Field</h3>'
		}
	});

	$("#Search").keyup(function (event)
	{
		if (event.keyCode === 13)
		{
			var entityName = $('#Search')[0].value;
			GetEntityByName(entityName);
			//drawCode(entityName);
		}
	});

	function GetEntityByName(name) {
	    $.ajax({
	        url: "/Home/SearchResult",
	        data: { name: name },
	        async: false,
	        success: function (data) {
	            $("#TableContent").html(data);
	            hljs.configure({ useBR: true });
	            //hljs.configure({ useBR: true });
	            $('div.code').each(function (i, block) {
	                hljs.highlightBlock(block);
	            });
	        }
	    });
	};

	function drawCode(name)
	{
	    $.ajax({
	        url: "/Home/returnXmlString",
	        data: { name: name },
	        async: false,
	        success: function (data) {
	            $("#codeBlock")[0].innerHTML=data;
	            hljs.configure({ useBR: true });
	            $('div.code').each(function (i, block) {
	                hljs.highlightBlock(block);
	            });
	        }
	    });

	}

});
